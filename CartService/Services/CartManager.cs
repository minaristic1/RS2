using CartService.Domain1;
using CartService.DTOs;
using CartService.Repositories;
using CartService.Exceptions;
using CartService.Clients;

namespace CartService.Services;

public class CartManager : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IRestaurantClient _restaurantClient;

    public CartManager(ICartRepository cartRepository, IRestaurantClient restaurantClient)
    {
        _cartRepository = cartRepository;
        _restaurantClient = restaurantClient;
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
        var menuItem = await _restaurantClient.GetMenuItemAsync(request.ProductId);

        if (menuItem is null)
        {
            throw new NotFoundException("Proizvod ne postoji.");
        }

        if (!menuItem.IsAvailable)
        {
            throw new ConflictException("Proizvod trenutno nije dostupan.");
        }
        
        var cart = await _cartRepository.GetCartAsync(userId);
        
        if (cart is null)
        {
            cart = new Cart
            {
                UserId = userId
            };
        }

        if (cart.Items.Count > 0)
        {
            var currentRestaurantId = cart.Items.First().RestaurantId;
            if (currentRestaurantId != menuItem.RestaurantId)
            {
                throw new ConflictException("Korpa može sadržati samo proizvode iz jednog restorana.");
            }
        }

        var existingItem = cart.Items.FirstOrDefault(item => item.ProductId == request.ProductId);

        if (existingItem is not null)
        {
            existingItem.Quantity += request.Quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = menuItem.Id,
                RestaurantId = menuItem.RestaurantId,
                ProductName = menuItem.Name,
                Price = menuItem.Price,
                Quantity = request.Quantity
            });
        }

        await _cartRepository.SaveCartAsync(cart);

        return MapToResponse(cart);
    }

    public async Task<CartResponse> UpdateItemQuantityAsync(Guid userId, Guid productId, UpdateCartItemRequest request)
    {
        var cart = await _cartRepository.GetCartAsync(userId);

        if (cart is null)
        {
            throw new NotFoundException("Korpa ne postoji.");
        }

        var item = cart.Items.FirstOrDefault(item => item.ProductId == productId);

        if (item is null)
        {
            throw new NotFoundException("Proizvod ne postoji u korpi.");
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
            throw new NotFoundException("Korpa ne postoji.");
        }

        var item = cart.Items.FirstOrDefault(item => item.ProductId == productId);

        if (item is null)
        {
            throw new NotFoundException("Proizvod ne postoji u korpi.");
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