using System.Net;
using System.Net.Http.Json;
using Billing.Application.Contracts.Infrastructure;
using Billing.Application.Models;

namespace Billing.API.Services;

public sealed class RestaurantServiceClient(HttpClient httpClient)
    : IRestaurantService
{
    public async Task<RestaurantInfo?> GetRestaurantAsync(
        Guid restaurantId,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync(
            $"api/restaurants/{restaurantId}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var restaurant = await response.Content.ReadFromJsonAsync<RestaurantResponse>(
            cancellationToken);

        return restaurant is null
            ? null
            : new RestaurantInfo(
                restaurant.Id,
                restaurant.NameSr,
                restaurant.Address);
    }

    private sealed record RestaurantResponse(
        Guid Id,
        string NameSr,
        string Address);
}

