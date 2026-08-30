using System.Text;
using System.Text.Json;
using Delivery.Api.Features.Deliveries.Commands.CreateDeliveryOrder;
using Delivery.Api.Messaging.Events;
using MediatR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Delivery.Api.Messaging
{
    public class OrderReadyForDeliveryConsumer : BackgroundService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public OrderReadyForDeliveryConsumer(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMq:Host"]!,
                Port = _configuration.GetValue<int>("RabbitMq:Port"),
                UserName = _configuration["RabbitMq:Username"]!,
                Password = _configuration["RabbitMq:Password"]!
            };

            await using var connection = await factory.CreateConnectionAsync(stoppingToken);
            await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

            const string exchangeName = "delivery.exchange";
            const string routingKey = "order.ready-for-delivery";
            const string queueName = "delivery.queue";

            await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct, durable: true, cancellationToken: stoppingToken);
            await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
            await channel.QueueBindAsync(queueName, exchangeName, routingKey, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var message = JsonSerializer.Deserialize<OrderReadyForDeliveryEvent>(json, JsonOptions);

                if (message != null)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    var command = new CreateDeliveryOrderCommand
                    {
                        OrderId = message.OrderId,
                        CustomerName = message.CustomerName,
                        CustomerPhone = message.CustomerPhone,
                        RestaurantId = message.RestaurantId,
                        RestaurantName = message.RestaurantName,
                        PickupAddress = message.PickupAddress,
                        DeliveryAddress = message.DeliveryAddress,
                        TotalPrice = message.TotalPrice,
                        Items = message.Items.Select(i => new CreateOrderItemDto
                        {
                            ProductName = i.ProductName,
                            Quantity = i.Quantity,
                            UnitPrice = i.UnitPrice
                        }).ToList()
                    };

                    await mediator.Send(command, stoppingToken);
                }

                await channel.BasicAckAsync(eventArgs.DeliveryTag, false, stoppingToken);
            };

            await channel.BasicConsumeAsync(queueName, autoAck: false, consumer, stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
