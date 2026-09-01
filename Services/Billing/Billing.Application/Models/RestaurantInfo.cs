namespace Billing.Application.Models;

public sealed record RestaurantInfo(
    Guid Id,
    string Name,
    string Address);

