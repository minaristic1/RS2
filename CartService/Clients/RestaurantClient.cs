using System.Net;
using System.Net.Http.Json;

namespace CartService.Clients;

public class RestaurantClient : IRestaurantClient 
{
    private readonly HttpClient _httpClient;

    public RestaurantClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<MenuItemResponse?> GetMenuItemAsync(Guid productId)
    {
        var response = await _httpClient.GetAsync($"api/menu-items/{productId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MenuItemResponse>();
    }
}