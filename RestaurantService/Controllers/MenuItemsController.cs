using Microsoft.AspNetCore.Mvc;
using RestaurantService.Application.Interfaces;

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
}