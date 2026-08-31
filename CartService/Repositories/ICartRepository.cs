using CartService.Domain1;

namespace CartService.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetCartAsync(Guid userId);
    
    Task<Cart> SaveCartAsync(Cart cart);
    
    Task DeleteCartAsync(Guid userId);
}