using EventBus.Messages.Events;

namespace CartService.Messaging;

public interface IRabbitMqPublisher
{
    Task PublishCartCheckedOutAsync(CartCheckedOutEvent message, CancellationToken cancellationToken = default);
}