using System.Text;
using System.Text.Json;
using Billing.Application.Contracts.Persistence;
using Billing.Application.Features.Billing.Commands.CreateInvoice;
using Billing.Domain.Exceptions;
using EventBus.Messages.Events;
using MediatR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Billing.API.Messaging;

public sealed class CartCheckedOutConsumer(
    IConfiguration configuration,
    IServiceScopeFactory scopeFactory,
    ILogger<CartCheckedOutConsumer> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMq:Host"]
                ?? throw new InvalidOperationException("RabbitMQ host is not configured."),
            Port = configuration.GetValue<int>("RabbitMq:Port"),
            UserName = configuration["RabbitMq:Username"]
                ?? throw new InvalidOperationException("RabbitMQ username is not configured."),
            Password = configuration["RabbitMq:Password"]
                ?? throw new InvalidOperationException("RabbitMQ password is not configured.")
        };

        await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(
            cancellationToken: stoppingToken);

        const string exchangeName = "cart.exchange";
        const string routingKey = "cart.checked-out";
        const string queueName = "payment.queue";

        await channel.ExchangeDeclareAsync(
            exchangeName,
            ExchangeType.Direct,
            durable: true,
            cancellationToken: stoppingToken);
        await channel.QueueDeclareAsync(
            queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);
        await channel.QueueBindAsync(
            queueName,
            exchangeName,
            routingKey,
            cancellationToken: stoppingToken);
        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var message = JsonSerializer.Deserialize<CartCheckedOutEvent>(
                    Encoding.UTF8.GetString(eventArgs.Body.Span),
                    JsonOptions)
                    ?? throw new JsonException("Checkout event body is empty.");

                await ProcessMessageAsync(message, stoppingToken);
                await channel.BasicAckAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    stoppingToken);
            }
            catch (Exception exception) when (
                exception is JsonException or BillingDomainException)
            {
                logger.LogError(
                    exception,
                    "Discarding invalid checkout event {DeliveryTag}.",
                    eventArgs.DeliveryTag);
                await channel.BasicRejectAsync(
                    eventArgs.DeliveryTag,
                    requeue: false,
                    stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "Failed to process checkout event {DeliveryTag}; requeueing it.",
                    eventArgs.DeliveryTag);
                await channel.BasicNackAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: true,
                    stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(
            queueName,
            autoAck: false,
            consumer,
            stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task ProcessMessageAsync(
        CartCheckedOutEvent message,
        CancellationToken cancellationToken)
    {
        if (message.OrderId == Guid.Empty)
        {
            throw new BillingDomainException("Checkout order identifier is required.");
        }

        await using var scope = scopeFactory.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();

        if (await repository.ExistsForOrderAsync(message.OrderId, cancellationToken))
        {
            logger.LogInformation(
                "Invoice for order {OrderId} already exists; acknowledging duplicate event.",
                message.OrderId);
            return;
        }

        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        var currency = configuration["Billing:DefaultCurrency"] ?? "RSD";
        var items = message.Items.Select(item => new CreateInvoiceItem(
            item.ProductId,
            item.ProductName,
            item.Quantity,
            item.Price)).ToArray();
        var calculatedTotal = items.Sum(item => item.Quantity * item.UnitPrice);

        if (calculatedTotal != message.TotalPrice)
        {
            throw new BillingDomainException(
                $"Checkout total {message.TotalPrice} does not match calculated total {calculatedTotal}.");
        }

        var invoice = await sender.Send(
            new CreateInvoiceCommand(
                message.OrderId,
                message.UserId,
                message.RestaurantId,
                message.DeliveryAddress,
                currency,
                items),
            cancellationToken);

        logger.LogInformation(
            "Created invoice {InvoiceId} for order {OrderId}.",
            invoice.Id,
            invoice.OrderId);
    }
}
