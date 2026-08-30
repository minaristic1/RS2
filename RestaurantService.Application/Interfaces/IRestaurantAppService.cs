using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;

using RestaurantService.Application.DTOs;

namespace RestaurantService.Application.Interfaces;

public interface IRestaurantAppService
{
    Task<List<RestaurantResponse>> GetAllAsync();

    Task<RestaurantResponse?> GetByIdAsync(Guid id);

    Task<MenuItemLookupResponse?> GetMenuItemByIdAsync(Guid id);

    Task<RestaurantMenuListResponse?> GetRestaurantMenuAsync(Guid restaurantId);

    Task<List<RestaurantResponse>> SearchByNameAsync(string searchTerm);

    Task<RestaurantResponse> CreateAsync(CreateRestaurantRequest request);

    Task<bool> UpdateAsync(Guid id, UpdateRestaurantRequest request);

    Task<bool> DeleteAsync(Guid id);

    Task<bool> UpdateMenuItemAsync(Guid id, UpdateMenuItemRequest request);

    Task<bool> DeleteMenuItemAsync(Guid id);

    Task<bool> SetOpeningHoursAsync(Guid restaurantId, List<OpeningHourEntryRequest> request);

    Task<MenuResponse?> CreateMenuAsync(Guid restaurantId, CreateMenuRequest request);

    Task<MenuCategoryResponse?> CreateMenuCategoryAsync(Guid restaurantId, Guid menuId, CreateMenuCategoryRequest request);

    Task<MenuItemSummaryResponse?> CreateMenuItemAsync(Guid restaurantId, Guid menuId, Guid categoryId, CreateMenuItemRequest request);
}