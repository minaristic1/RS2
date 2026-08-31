using CartService.DTOs;
using CartService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CartService.Controllers;

[ApiController]
[Authorize]
[Route("api/carts")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    
    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<CartResponse>> GetCart(Guid userId)
    {
        if (!CanAccess(userId))
        {
            return Forbid();
        }

        var cart = await _cartService.GetCartAsync(userId);

        return Ok(cart);
    }

    [HttpPost("{userId:guid}/items")]
    public async Task<ActionResult<CartResponse>> AddItem(Guid userId, [FromBody] AddCartItemRequest request)
    {
        if (!CanAccess(userId))
        {
            return Forbid();
        }

        var cart = await _cartService.AddItemAsync(userId, request);

        return Ok(cart);
    }

    [HttpPut("{userId:guid}/items/{productId:guid}")]
    public async Task<ActionResult<CartResponse>> UpdateItemQuantity(Guid userId, Guid productId,
        [FromBody] UpdateCartItemRequest request)
    {
        if (!CanAccess(userId))
        {
            return Forbid();
        }

        var cart = await _cartService.UpdateItemQuantityAsync(userId, productId, request);

        return Ok(cart);
    }

    [HttpDelete("{userId:guid}/items/{productId:guid}")]
    public async Task<ActionResult<CartResponse>> RemoveItem(Guid userId, Guid productId)
    {
        if (!CanAccess(userId))
        {
            return Forbid();
        }

        var cart = await _cartService.RemoveItemAsync(userId, productId);
        return Ok(cart);
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> ClearCart(Guid userId)
    {
        if (!CanAccess(userId))
        {
            return Forbid();
        }

        await _cartService.ClearCartAsync(userId);
        return NoContent();
    }
    
    [HttpPost("{userId:guid}/checkout")]
    public async Task<IActionResult> Checkout(Guid userId, [FromBody] CheckoutRequest request)
    {
        if (!CanAccess(userId))
        {
            return Forbid();
        }

        await _cartService.CheckoutAsync(userId, request);

        return Accepted();
    }

    private bool CanAccess(Guid userId)
    {
        if (User.IsInRole("Admin"))
        {
            return true;
        }

        return Guid.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var currentUserId)
            && currentUserId == userId;
    }
}