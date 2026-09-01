using Billing.Application.Models;
using Billing.Domain.Aggregates;

namespace Billing.Application.Contracts.Infrastructure;

public interface IOrderReadyForDeliveryPublisher
{
    Task PublishAsync(
        Invoice invoice,
        RestaurantInfo restaurant,
        string customerName,
        string customerPhone,
        CancellationToken cancellationToken = default);
}

