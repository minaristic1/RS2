using CartService.DTOs;
using CartService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Controllers;

[ApiController]
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
        var cart = await _cartService.GetCartAsync(userId);

        return Ok(cart);
    }

    [HttpPost("{userId:guid}/items")]
    public async Task<ActionResult<CartResponse>> AddItem(Guid userId, [FromBody] AddCartItemRequest request)
    {
        var cart = await _cartService.AddItemAsync(userId, request);

        return Ok(cart);
    }

    [HttpPut("{userId:guid}/items/{productId:guid}")]
    public async Task<ActionResult<CartResponse>> UpdateItemQuantity(Guid userId, Guid productId,
        [FromBody] UpdateCartItemRequest request)
    {
        var cart = await _cartService.UpdateItemQuantityAsync(userId, productId, request);

        return Ok(cart);
    }

    [HttpDelete("{userId:guid}/items/{productId:guid}")]
    public async Task<ActionResult<CartResponse>> RemoveItem(Guid userId, Guid productId)
    {
        var cart = await _cartService.RemoveItemAsync(userId, productId);
        return Ok(cart);
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> ClearCart(Guid userId)
    {
        await _cartService.ClearCartAsync(userId);
        return NoContent();
    }
}