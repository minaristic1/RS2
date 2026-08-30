using CartService.DTOs;

namespace CartService.Services;

public interface ICartService
{
    Task<CartResponse> GetCartAsync(Guid userId);
    
    Task<CartResponse> AddItemAsync(Guid userId, AddCartItemRequest request);
    
    Task<CartResponse> UpdateItemQuantityAsync(Guid userId, Guid productId, UpdateCartItemRequest request);
    
    Task<CartResponse> RemoveItemAsync(Guid userId, Guid productId);
    
    Task ClearCartAsync(Guid userId);

    Task CheckoutAsync(Guid userId, CheckoutRequest request);
}