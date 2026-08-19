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
            .ToListAsync();
    }

    public async Task<Restaurant?> GetByIdAsync(Guid id)
    {
        return await _context.Restaurants
            .FirstOrDefaultAsync(restaurant => restaurant.Id == id);
    }

    public async Task<MenuItem?> GetMenuItemByIdAsync(Guid id)
    {
        return await _context.MenuItems
            .AsNoTracking()
            .FirstOrDefaultAsync(menuItem => menuItem.Id == id);
    }

    public async Task<List<Restaurant>> SearchByNameAsync(string searchTerm)
    {
        return await _context.Restaurants
            .AsNoTracking()
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
}