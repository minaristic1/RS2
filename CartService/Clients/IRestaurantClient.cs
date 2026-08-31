namespace CartService.Clients;

public interface IRestaurantClient
{
    Task<MenuItemResponse?> GetMenuItemAsync(Guid productId);
}