using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using RestaurantService.Application.Common;
using RestaurantService.Application.DTOs;
using RestaurantService.Application.Interfaces;
using RestaurantService.Extensions;

namespace RestaurantService.Controllers;

[ApiController]
[Route("api/restaurants")]
public class RestaurantsController : ControllerBase
{
    private readonly IRestaurantAppService _restaurantAppService;

    public RestaurantsController(IRestaurantAppService restaurantAppService)
    {
        _restaurantAppService = restaurantAppService;
    }

    [HttpGet]
    public async Task<ActionResult<List<RestaurantResponse>>> GetAll()
    {
        var restaurants = await _restaurantAppService.GetAllAsync();
        return Ok(restaurants);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RestaurantResponse>> GetById(Guid id)
    {
        var restaurant = await _restaurantAppService.GetByIdAsync(id);

        if (restaurant is null)
        {
            return NotFound();
        }

        return Ok(restaurant);
    }

    [HttpGet("{restaurantId:guid}/menu")]
    public async Task<ActionResult<RestaurantMenuListResponse>> GetMenu(Guid restaurantId)
    {
        var menu = await _restaurantAppService.GetRestaurantMenuAsync(restaurantId);

        if (menu is null)
        {
            return NotFound();
        }

        return Ok(menu);
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<RestaurantResponse>>> Search([FromQuery] string term)
    {
        var restaurants = await _restaurantAppService.SearchByNameAsync(term);
        return Ok(restaurants);
    }

    [Authorize(Roles = "RestaurantOwner,Admin")]
    [HttpPost]
    public async Task<ActionResult<RestaurantResponse>> Create([FromBody] CreateRestaurantRequest request)
    {
        var ownerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var restaurant = await _restaurantAppService.CreateAsync(request, ownerId);
        return CreatedAtAction(nameof(GetById), new { id = restaurant.Id }, restaurant);
    }

    [Authorize(Roles = "RestaurantOwner,Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRestaurantRequest request)
    {
        var result = await _restaurantAppService.UpdateAsync(id, request, User.ToRequestingUser());

        return result.Status switch
        {
            ServiceStatus.NotFound => NotFound(),
            ServiceStatus.Forbidden => Forbid(),
            _ => NoContent()
        };
    }

    [Authorize(Roles = "RestaurantOwner,Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _restaurantAppService.DeleteAsync(id, User.ToRequestingUser());

        return result.Status switch
        {
            ServiceStatus.NotFound => NotFound(),
            ServiceStatus.Forbidden => Forbid(),
            _ => NoContent()
        };
    }

    [Authorize(Roles = "RestaurantOwner,Admin")]
    [HttpPut("{id:guid}/opening-hours")]
    public async Task<IActionResult> SetOpeningHours(Guid id, [FromBody] List<OpeningHourEntryRequest> request)
    {
        var result = await _restaurantAppService.SetOpeningHoursAsync(id, request, User.ToRequestingUser());

        return result.Status switch
        {
            ServiceStatus.NotFound => NotFound(),
            ServiceStatus.Forbidden => Forbid(),
            _ => NoContent()
        };
    }

    [Authorize(Roles = "RestaurantOwner,RestaurantEmployee,Admin")]
    [HttpPost("{restaurantId:guid}/menus")]
    public async Task<ActionResult<MenuResponse>> CreateMenu(Guid restaurantId, [FromBody] CreateMenuRequest request)
    {
        var result = await _restaurantAppService.CreateMenuAsync(restaurantId, request, User.ToRequestingUser());

        return result.Status switch
        {
            ServiceStatus.NotFound => NotFound(),
            ServiceStatus.Forbidden => Forbid(),
            _ => StatusCode(StatusCodes.Status201Created, result.Value)
        };
    }

    [Authorize(Roles = "RestaurantOwner,RestaurantEmployee,Admin")]
    [HttpPost("{restaurantId:guid}/menus/{menuId:guid}/categories")]
    public async Task<ActionResult<MenuCategoryResponse>> CreateMenuCategory(Guid restaurantId, Guid menuId, [FromBody] CreateMenuCategoryRequest request)
    {
        var result = await _restaurantAppService.CreateMenuCategoryAsync(restaurantId, menuId, request, User.ToRequestingUser());

        return result.Status switch
        {
            ServiceStatus.NotFound => NotFound(),
            ServiceStatus.Forbidden => Forbid(),
            _ => StatusCode(StatusCodes.Status201Created, result.Value)
        };
    }

    [Authorize(Roles = "RestaurantOwner,RestaurantEmployee,Admin")]
    [HttpPost("{restaurantId:guid}/menus/{menuId:guid}/categories/{categoryId:guid}/items")]
    public async Task<ActionResult<MenuItemSummaryResponse>> CreateMenuItem(Guid restaurantId, Guid menuId, Guid categoryId, [FromBody] CreateMenuItemRequest request)
    {
        var result = await _restaurantAppService.CreateMenuItemAsync(restaurantId, menuId, categoryId, request, User.ToRequestingUser());

        return result.Status switch
        {
            ServiceStatus.NotFound => NotFound(),
            ServiceStatus.Forbidden => Forbid(),
            _ => StatusCode(StatusCodes.Status201Created, result.Value)
        };
    }
}
