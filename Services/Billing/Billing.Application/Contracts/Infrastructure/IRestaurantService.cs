using Billing.Application.Models;

namespace Billing.Application.Contracts.Infrastructure;

public interface IRestaurantService
{
    Task<RestaurantInfo?> GetRestaurantAsync(
        Guid restaurantId,
        CancellationToken cancellationToken = default);
}

