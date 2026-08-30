using Microsoft.AspNetCore.Mvc;
using RestaurantService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using RestaurantService.Application.DTOs;

namespace RestaurantService.Controllers;

[ApiController]
[Route("api/menu-items")]
public class MenuItemsController : ControllerBase
{
    private readonly IRestaurantAppService _restaurantAppService;

    public MenuItemsController(IRestaurantAppService restaurantAppService)
    {
        _restaurantAppService = restaurantAppService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var menuItem = await _restaurantAppService.GetMenuItemByIdAsync(id);

        if (menuItem is null)
        {
            return NotFound();
        }

        return Ok(menuItem);
    }

    [Authorize(Roles = "RestaurantOwner,RestaurantEmployee,Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMenuItemRequest request)
    {
        var success = await _restaurantAppService.UpdateMenuItemAsync(id, request);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize(Roles = "RestaurantOwner,RestaurantEmployee,Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _restaurantAppService.DeleteMenuItemAsync(id);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}