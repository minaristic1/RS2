using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using RestaurantService.Application.Interfaces;
using RestaurantService.Domain.Entities;
using RestaurantService.Infrastructure.Persistence;

namespace RestaurantService.Infrastructure.Repositories;

public class RestaurantRepository : IRestaurantRepository
{
    private readonly RestaurantDbContext _context;

    public RestaurantRepository(RestaurantDbContext context)
    {
        _context = context;
    }

    public async Task<List<Restaurant>> GetAllAsync()
    {
        return await _context.Restaurants
            .AsNoTracking()
            .Include(restaurant => restaurant.OpeningHours)
            .Include(restaurant => restaurant.HolidayExceptions)
            .ToListAsync();
    }

    public async Task<Restaurant?> GetByIdAsync(Guid id)
    {
        return await _context.Restaurants
            .Include(restaurant => restaurant.OpeningHours)
            .Include(restaurant => restaurant.HolidayExceptions)
            .FirstOrDefaultAsync(restaurant => restaurant.Id == id);
    }

    public async Task<MenuItem?> GetMenuItemByIdAsync(Guid id)
    {
        return await _context.MenuItems
            .AsNoTracking()
            .FirstOrDefaultAsync(menuItem => menuItem.Id == id);
    }

    public async Task<MenuItem?> GetTrackedMenuItemByIdAsync(Guid id)
    {
        return await _context.MenuItems
            .FirstOrDefaultAsync(menuItem => menuItem.Id == id);
    }

    public async Task DeleteMenuItemAsync(Guid id)
    {
        var menuItem = await _context.MenuItems.FindAsync(id);

        if (menuItem is not null)
        {
            _context.MenuItems.Remove(menuItem);
        }
    }

    public async Task<List<Menu>> GetMenusByRestaurantIdAsync(Guid restaurantId)
    {
        return await _context.Menus
            .AsNoTracking()
            .Where(menu => menu.RestaurantId == restaurantId && menu.IsActive)
            .OrderBy(menu => menu.DisplayOrder)
            .Include(menu => menu.Categories
                .Where(category => category.IsActive)
                .OrderBy(category => category.DisplayOrder))
            .ThenInclude(category => category.Items)
            .ToListAsync();
    }

    public async Task<List<Restaurant>> SearchByNameAsync(string searchTerm)
    {
        return await _context.Restaurants
            .AsNoTracking()
            .Include(restaurant => restaurant.OpeningHours)
            .Include(restaurant => restaurant.HolidayExceptions)
            .Where(restaurant =>
                restaurant.NameSr.Contains(searchTerm) ||
                restaurant.NameEn.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task AddAsync(Restaurant restaurant)
    {
        await _context.Restaurants.AddAsync(restaurant);
    }

    public Task UpdateAsync(Restaurant restaurant)
    {
        _context.Restaurants.Update(restaurant);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var restaurant = await _context.Restaurants.FindAsync(id);

        if (restaurant is not null)
        {
            _context.Restaurants.Remove(restaurant);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task ReplaceOpeningHoursAsync(Guid restaurantId, List<RestaurantOpeningHours> newHours)
    {
        var existing = await _context.RestaurantOpeningHours
            .Where(openingHours => openingHours.RestaurantId == restaurantId)
            .ToListAsync();

        _context.RestaurantOpeningHours.RemoveRange(existing);
        await _context.RestaurantOpeningHours.AddRangeAsync(newHours);
    }

    public async Task<bool> RestaurantExistsAsync(Guid restaurantId)
    {
        return await _context.Restaurants.AnyAsync(restaurant => restaurant.Id == restaurantId);
    }

    public async Task<Menu?> GetMenuByIdAsync(Guid menuId)
    {
        return await _context.Menus.FirstOrDefaultAsync(menu => menu.Id == menuId);
    }

    public async Task<MenuCategory?> GetMenuCategoryByIdAsync(Guid categoryId)
    {
        return await _context.MenuCategories.FirstOrDefaultAsync(category => category.Id == categoryId);
    }

    public async Task AddMenuAsync(Menu menu)
    {
        await _context.Menus.AddAsync(menu);
    }

    public async Task AddMenuCategoryAsync(MenuCategory category)
    {
        await _context.MenuCategories.AddAsync(category);
    }

    public async Task AddMenuItemAsync(MenuItem item)
    {
        await _context.MenuItems.AddAsync(item);
    }
}