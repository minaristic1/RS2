using CartService.Domain1;
using CartService.DTOs;
using CartService.Repositories;

namespace CartService.Services;

public class CartManager : ICartService
{
    private readonly ICartRepository _cartRepository;

    public CartManager(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<CartResponse> GetCartAsync(Guid userId)
    {
        var cart = await _cartRepository.GetCartAsync(userId);

        if (cart is null)
        {
            cart = new Cart
            {
                UserId = userId
            };
        }

        return MapToResponse(cart);
    }

    public async Task<CartResponse> AddItemAsync(Guid userId, AddCartItemRequest request)
    {
        var cart = await _cartRepository.GetCartAsync(userId);
        
        if (cart is null)
        {
            cart = new Cart
            {
                UserId = userId
            };
        }

        var existingItem = cart.Items.FirstOrDefault(item => item.ProductId == request.ProductId);

        if (existingItem is not null)
        {
            existingItem.Quantity += request.Quantity;
        }
        else
        {
            cart.Items.Add(new CartItem{ProductId = request.ProductId, Quantity = request.Quantity});
        }

        await _cartRepository.SaveCartAsync(cart);

        return MapToResponse(cart);
    }

    public async Task<CartResponse> UpdateItemQuantityAsync(Guid userId, Guid productId, UpdateCartItemRequest request)
    {
        var cart = await _cartRepository.GetCartAsync(userId);

        if (cart is null)
        {
            throw new InvalidOperationException("Korpa ne postoji.");
        }

        var item = cart.Items.FirstOrDefault(item => item.ProductId == productId);

        if (item is null)
        {
            throw new InvalidOperationException("Proizvod ne postoji u korpi.");
        }

        item.Quantity = request.Quantity;

        await _cartRepository.SaveCartAsync(cart);
        
        return MapToResponse(cart);
    }

    public async Task<CartResponse> RemoveItemAsync(Guid userId, Guid productId)
    {
        var cart = await _cartRepository.GetCartAsync(userId);

        if (cart is null)
        {
            throw new InvalidOperationException("Korpa ne postoji.");
        }

        var item = cart.Items.FirstOrDefault(item => item.ProductId == productId);

        if (item is null)
        {
            throw new InvalidOperationException("Proizvod ne postoji u korpi.");
        }

        cart.Items.Remove(item);

        await _cartRepository.SaveCartAsync(cart);

        return MapToResponse(cart);
    }

    public async Task ClearCartAsync(Guid userId)
    {
        await _cartRepository.DeleteCartAsync(userId);
    }

    private static CartResponse MapToResponse(Cart cart)
    {
        return new CartResponse
        {
            UserId = cart.UserId,
            TotalPrice = cart.TotalPrice,
            Items = cart.Items.Select(item => new CartItemResponse
            {
                ProductId = item.ProductId,
                RestaurantId = item.RestaurantId,
                ProductName = item.ProductName,
                Price = item.Price,
                Quantity = item.Quantity
            }).ToList()
        };
    }
}