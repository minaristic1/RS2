using System.Text;
using System.Text.Json;
using EventBus.Messages.Events;
using RabbitMQ.Client;
 
namespace CartService.Messaging;
 
public class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly IConfiguration _configuration;
    
    public RabbitMqPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
    }
 
    public async Task PublishCartCheckedOutAsync(CartCheckedOutEvent message, CancellationToken cancellationToken = default)
    {
        var host = _configuration["RabbitMq:Host"]
                   ?? throw new InvalidOperationException("RabbitMQ host is not configured.");
 
        var username = _configuration["RabbitMq:Username"]
                       ?? throw new InvalidOperationException("RabbitMQ username is not configured.");
 
        var password = _configuration["RabbitMq:Password"]
                       ?? throw new InvalidOperationException("RabbitMQ password is not configured.");
 
        var port = _configuration.GetValue<int>("RabbitMq:Port");
 
        var factory = new ConnectionFactory
        {
            HostName = host,
            Port = port,
            UserName = username,
            Password = password
        };
 
        await using var connection = await factory.CreateConnectionAsync(cancellationToken);
 
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
 
        const string exchangeName = "cart.exchange";
        const string routingKey = "cart.checked-out";
        const string queueName = "payment.queue";
 
        await channel.ExchangeDeclareAsync(
            exchange: exchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);
        
        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);
        
        await channel.QueueBindAsync(
            queue: queueName,
            exchange: exchangeName,
            routingKey: routingKey,
            arguments: null,
            cancellationToken: cancellationToken);
 
        var json = JsonSerializer.Serialize(message);
 
        var body = Encoding.UTF8.GetBytes(json);
 
        await channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: routingKey,
            mandatory: false,
            body: body,
            cancellationToken: cancellationToken);
    }
}