using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using RestaurantService.Application.DTOs;
using RestaurantService.Application.Interfaces;

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

    [HttpPost]
    public async Task<ActionResult<RestaurantResponse>> Create([FromBody] CreateRestaurantRequest request)
    {
        var restaurant = await _restaurantAppService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = restaurant.Id }, restaurant);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRestaurantRequest request)
    {
        var success = await _restaurantAppService.UpdateAsync(id, request);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _restaurantAppService.DeleteAsync(id);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}