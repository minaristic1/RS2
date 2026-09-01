using System.Text;
using System.Text.Json;
using Billing.Application.Contracts.Infrastructure;
using Billing.Application.Models;
using Billing.Domain.Aggregates;
using EventBus.Messages.Events;
using RabbitMQ.Client;

namespace Billing.API.Messaging;

public sealed class OrderReadyForDeliveryPublisher(IConfiguration configuration)
    : IOrderReadyForDeliveryPublisher
{
    public async Task PublishAsync(
        Invoice invoice,
        RestaurantInfo restaurant,
        string customerName,
        string customerPhone,
        CancellationToken cancellationToken = default)
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

        await using var connection = await factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(
            cancellationToken: cancellationToken);

        const string exchangeName = "delivery.exchange";
        const string routingKey = "order.ready-for-delivery";
        const string queueName = "delivery.queue";

        await channel.ExchangeDeclareAsync(
            exchangeName,
            ExchangeType.Direct,
            durable: true,
            cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync(
            queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);
        await channel.QueueBindAsync(
            queueName,
            exchangeName,
            routingKey,
            cancellationToken: cancellationToken);

        var message = new OrderReadyForDeliveryEvent
        {
            OrderId = invoice.OrderId,
            CustomerName = customerName,
            CustomerPhone = customerPhone,
            RestaurantId = invoice.RestaurantId,
            RestaurantName = restaurant.Name,
            PickupAddress = restaurant.Address,
            DeliveryAddress = invoice.DeliveryAddress,
            TotalPrice = invoice.TotalAmount,
            Items = invoice.Items.Select(item => new OrderReadyForDeliveryItem
            {
                ProductName = item.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        await channel.BasicPublishAsync(
            exchangeName,
            routingKey,
            mandatory: false,
            body,
            cancellationToken);
    }
}

