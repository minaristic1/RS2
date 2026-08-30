using Microsoft.AspNetCore.Mvc;
using RestaurantService.Application.Common;
using RestaurantService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using RestaurantService.Application.DTOs;
using RestaurantService.Extensions;

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
        var result = await _restaurantAppService.UpdateMenuItemAsync(id, request, User.ToRequestingUser());

        return result.Status switch
        {
            ServiceStatus.NotFound => NotFound(),
            ServiceStatus.Forbidden => Forbid(),
            _ => NoContent()
        };
    }

    [Authorize(Roles = "RestaurantOwner,RestaurantEmployee,Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _restaurantAppService.DeleteMenuItemAsync(id, User.ToRequestingUser());

        return result.Status switch
        {
            ServiceStatus.NotFound => NotFound(),
            ServiceStatus.Forbidden => Forbid(),
            _ => NoContent()
        };
    }
}
