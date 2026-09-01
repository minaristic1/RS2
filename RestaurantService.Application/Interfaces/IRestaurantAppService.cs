using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;

using RestaurantService.Application.Common;
using RestaurantService.Application.DTOs;
using RestaurantService.Application.Security;

namespace RestaurantService.Application.Interfaces;

public interface IRestaurantAppService
{
    Task<List<RestaurantResponse>> GetAllAsync();

    Task<RestaurantResponse?> GetByIdAsync(Guid id);

    Task<MenuItemLookupResponse?> GetMenuItemByIdAsync(Guid id);

    Task<RestaurantMenuListResponse?> GetRestaurantMenuAsync(Guid restaurantId);

    Task<List<RestaurantResponse>> SearchByNameAsync(string searchTerm);

    Task<RestaurantResponse> CreateAsync(CreateRestaurantRequest request, Guid ownerId);

    Task<ServiceResult> UpdateAsync(Guid id, UpdateRestaurantRequest request, RequestingUser requestingUser);

    Task<ServiceResult> DeleteAsync(Guid id, RequestingUser requestingUser);

    Task<ServiceResult> UpdateMenuItemAsync(Guid id, UpdateMenuItemRequest request, RequestingUser requestingUser);

    Task<ServiceResult> DeleteMenuItemAsync(Guid id, RequestingUser requestingUser);

    Task<ServiceResult> SetOpeningHoursAsync(Guid restaurantId, List<OpeningHourEntryRequest> request, RequestingUser requestingUser);

    Task<ServiceResult<MenuResponse>> CreateMenuAsync(Guid restaurantId, CreateMenuRequest request, RequestingUser requestingUser);

    Task<ServiceResult<MenuCategoryResponse>> CreateMenuCategoryAsync(Guid restaurantId, Guid menuId, CreateMenuCategoryRequest request, RequestingUser requestingUser);

    Task<ServiceResult<MenuItemSummaryResponse>> CreateMenuItemAsync(Guid restaurantId, Guid menuId, Guid categoryId, CreateMenuItemRequest request, RequestingUser requestingUser);

    Task<ServiceResult<MenuResponse>> UpdateMenuAsync(Guid restaurantId, Guid menuId, UpdateMenuRequest request, RequestingUser requestingUser);

    Task<ServiceResult> DeleteMenuAsync(Guid restaurantId, Guid menuId, RequestingUser requestingUser);

    Task<ServiceResult<MenuCategoryResponse>> UpdateMenuCategoryAsync(Guid restaurantId, Guid menuId, Guid categoryId, UpdateMenuCategoryRequest request, RequestingUser requestingUser);

    Task<ServiceResult> DeleteMenuCategoryAsync(Guid restaurantId, Guid menuId, Guid categoryId, RequestingUser requestingUser);

    Task<List<HolidayExceptionResponse>?> GetHolidayExceptionsAsync(Guid restaurantId);

    Task<ServiceResult<HolidayExceptionResponse>> CreateHolidayExceptionAsync(Guid restaurantId, CreateHolidayExceptionRequest request, RequestingUser requestingUser);

    Task<ServiceResult> DeleteHolidayExceptionAsync(Guid restaurantId, Guid exceptionId, RequestingUser requestingUser);
}
