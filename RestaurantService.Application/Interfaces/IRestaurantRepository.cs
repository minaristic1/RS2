using System;
using System.Collections.Generic;
using System.Text;

using RestaurantService.Domain.Entities;

namespace RestaurantService.Application.Interfaces;

public interface IRestaurantRepository
{
    Task<List<Restaurant>> GetAllAsync();

    Task<Restaurant?> GetByIdAsync(Guid id);

    Task<MenuItem?> GetMenuItemByIdAsync(Guid id);

    Task<MenuItem?> GetTrackedMenuItemByIdAsync(Guid id);

    Task DeleteMenuItemAsync(Guid id);

    Task<List<Menu>> GetMenusByRestaurantIdAsync(Guid restaurantId);

    Task<List<Restaurant>> SearchByNameAsync(string searchTerm);

    Task AddAsync(Restaurant restaurant);

    Task UpdateAsync(Restaurant restaurant);

    Task DeleteAsync(Guid id);

    Task SaveChangesAsync();
}
