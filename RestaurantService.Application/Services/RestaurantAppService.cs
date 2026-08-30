using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

using RestaurantService.Application.Common;
using RestaurantService.Application.DTOs;
using RestaurantService.Application.Interfaces;
using RestaurantService.Application.Security;
using RestaurantService.Domain.Entities;

namespace RestaurantService.Application.Services;

public class RestaurantAppService : IRestaurantAppService
{
    private readonly IRestaurantRepository _repository;

    public RestaurantAppService(IRestaurantRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<RestaurantResponse>> GetAllAsync()
    {
        var restaurants = await _repository.GetAllAsync();
        return restaurants.Select(MapToResponse).ToList();
    }

    public async Task<RestaurantResponse?> GetByIdAsync(Guid id)
    {
        var restaurant = await _repository.GetByIdAsync(id);
        return restaurant is null ? null : MapToResponse(restaurant);
    }

    public async Task<List<RestaurantResponse>> SearchByNameAsync(string searchTerm)
    {
        var restaurants = await _repository.SearchByNameAsync(searchTerm);
        return restaurants.Select(MapToResponse).ToList();
    }

    public async Task<MenuItemLookupResponse?> GetMenuItemByIdAsync(Guid id)
    {
        var menuItem = await _repository.GetMenuItemByIdAsync(id);

        if (menuItem is null)
        {
            return null;
        }

        return new MenuItemLookupResponse
        {
            Id = menuItem.Id,
            RestaurantId = menuItem.RestaurantId,
            Name = menuItem.NameSr,
            Price = menuItem.Price,
            IsAvailable = menuItem.IsAvailable
        };
    }

    public async Task<RestaurantMenuListResponse?> GetRestaurantMenuAsync(Guid restaurantId)
    {
        var restaurant = await _repository.GetByIdAsync(restaurantId);

        if (restaurant is null)
        {
            return null;
        }

        var menus = await _repository.GetMenusByRestaurantIdAsync(restaurantId);

        return new RestaurantMenuListResponse
        {
            RestaurantId = restaurantId,
            Menus = menus.Select(MapToMenuSummary).ToList()
        };
    }

    public async Task<RestaurantResponse> CreateAsync(CreateRestaurantRequest request, Guid ownerId)
    {
        var restaurant = new Restaurant
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            NameSr = request.NameSr,
            NameEn = request.NameEn,
            DescriptionSr = request.DescriptionSr,
            DescriptionEn = request.DescriptionEn,
            Address = request.Address,
            ImageUrl = request.ImageUrl,
            IsFeatured = request.IsFeatured,
            CuisineType = request.CuisineType,
            IsActive = true
        };

        await _repository.AddAsync(restaurant);
        await _repository.SaveChangesAsync();

        return MapToResponse(restaurant);
    }

    public async Task<ServiceResult> UpdateAsync(Guid id, UpdateRestaurantRequest request, RequestingUser requestingUser)
    {
        var restaurant = await _repository.GetByIdAsync(id);

        if (restaurant is null)
        {
            return ServiceResult.NotFound();
        }

        if (!requestingUser.CanManageRestaurant(restaurant.Id, restaurant.OwnerId))
        {
            return ServiceResult.Forbidden();
        }

        restaurant.NameSr = request.NameSr;
        restaurant.NameEn = request.NameEn;
        restaurant.DescriptionSr = request.DescriptionSr;
        restaurant.DescriptionEn = request.DescriptionEn;
        restaurant.Address = request.Address;
        restaurant.ImageUrl = request.ImageUrl;
        restaurant.IsActive = request.IsActive;
        restaurant.IsFeatured = request.IsFeatured;
        restaurant.CuisineType = request.CuisineType;

        await _repository.UpdateAsync(restaurant);
        await _repository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(Guid id, RequestingUser requestingUser)
    {
        var restaurant = await _repository.GetByIdAsync(id);

        if (restaurant is null)
        {
            return ServiceResult.NotFound();
        }

        if (!requestingUser.CanManageRestaurant(restaurant.Id, restaurant.OwnerId))
        {
            return ServiceResult.Forbidden();
        }

        await _repository.DeleteAsync(id);
        await _repository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> UpdateMenuItemAsync(Guid id, UpdateMenuItemRequest request, RequestingUser requestingUser)
    {
        var menuItem = await _repository.GetTrackedMenuItemByIdAsync(id);

        if (menuItem is null)
        {
            return ServiceResult.NotFound();
        }

        var restaurant = await _repository.GetByIdAsync(menuItem.RestaurantId);

        if (restaurant is null)
        {
            return ServiceResult.NotFound();
        }

        if (!requestingUser.CanManageRestaurant(restaurant.Id, restaurant.OwnerId))
        {
            return ServiceResult.Forbidden();
        }

        menuItem.NameSr = request.NameSr;
        menuItem.NameEn = request.NameEn;
        menuItem.DescriptionSr = request.DescriptionSr;
        menuItem.DescriptionEn = request.DescriptionEn;
        menuItem.Price = request.Price;
        menuItem.ImageUrl = request.ImageUrl;
        menuItem.IsAvailable = request.IsAvailable;
        menuItem.IsFeatured = request.IsFeatured;
        menuItem.PreparationTimeMinutes = request.PreparationTimeMinutes;

        await _repository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteMenuItemAsync(Guid id, RequestingUser requestingUser)
    {
        var menuItem = await _repository.GetTrackedMenuItemByIdAsync(id);

        if (menuItem is null)
        {
            return ServiceResult.NotFound();
        }

        var restaurant = await _repository.GetByIdAsync(menuItem.RestaurantId);

        if (restaurant is null)
        {
            return ServiceResult.NotFound();
        }

        if (!requestingUser.CanManageRestaurant(restaurant.Id, restaurant.OwnerId))
        {
            return ServiceResult.Forbidden();
        }

        await _repository.DeleteMenuItemAsync(id);
        await _repository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> SetOpeningHoursAsync(Guid restaurantId, List<OpeningHourEntryRequest> request, RequestingUser requestingUser)
    {
        var restaurant = await _repository.GetByIdAsync(restaurantId);

        if (restaurant is null)
        {
            return ServiceResult.NotFound();
        }

        if (!requestingUser.CanManageRestaurant(restaurant.Id, restaurant.OwnerId))
        {
            return ServiceResult.Forbidden();
        }

        var newHours = request.Select(entry => new RestaurantOpeningHours
        {
            Id = Guid.NewGuid(),
            RestaurantId = restaurantId,
            DayOfWeek = entry.DayOfWeek,
            OpenTime = TimeSpan.Parse(entry.OpenTime),
            CloseTime = TimeSpan.Parse(entry.CloseTime),
            IsClosed = entry.IsClosed
        }).ToList();

        await _repository.ReplaceOpeningHoursAsync(restaurantId, newHours);
        await _repository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult<MenuResponse>> CreateMenuAsync(Guid restaurantId, CreateMenuRequest request, RequestingUser requestingUser)
    {
        var restaurant = await _repository.GetByIdAsync(restaurantId);

        if (restaurant is null)
        {
            return ServiceResult<MenuResponse>.NotFound();
        }

        if (!requestingUser.CanManageRestaurant(restaurant.Id, restaurant.OwnerId))
        {
            return ServiceResult<MenuResponse>.Forbidden();
        }

        var menu = new Menu
        {
            Id = Guid.NewGuid(),
            RestaurantId = restaurantId,
            NameSr = request.NameSr,
            NameEn = request.NameEn,
            DescriptionSr = request.DescriptionSr,
            DescriptionEn = request.DescriptionEn,
            DisplayOrder = request.DisplayOrder,
            IsActive = true
        };

        await _repository.AddMenuAsync(menu);
        await _repository.SaveChangesAsync();

        return ServiceResult<MenuResponse>.Success(new MenuResponse
        {
            Id = menu.Id,
            RestaurantId = menu.RestaurantId,
            NameSr = menu.NameSr,
            NameEn = menu.NameEn,
            DescriptionSr = menu.DescriptionSr,
            DescriptionEn = menu.DescriptionEn,
            DisplayOrder = menu.DisplayOrder,
            IsActive = menu.IsActive
        });
    }

    public async Task<ServiceResult<MenuCategoryResponse>> CreateMenuCategoryAsync(Guid restaurantId, Guid menuId, CreateMenuCategoryRequest request, RequestingUser requestingUser)
    {
        var menu = await _repository.GetMenuByIdAsync(menuId);

        if (menu is null || menu.RestaurantId != restaurantId)
        {
            return ServiceResult<MenuCategoryResponse>.NotFound();
        }

        var restaurant = await _repository.GetByIdAsync(restaurantId);

        if (restaurant is null)
        {
            return ServiceResult<MenuCategoryResponse>.NotFound();
        }

        if (!requestingUser.CanManageRestaurant(restaurant.Id, restaurant.OwnerId))
        {
            return ServiceResult<MenuCategoryResponse>.Forbidden();
        }

        var category = new MenuCategory
        {
            Id = Guid.NewGuid(),
            MenuId = menuId,
            NameSr = request.NameSr,
            NameEn = request.NameEn,
            DescriptionSr = request.DescriptionSr,
            DescriptionEn = request.DescriptionEn,
            DisplayOrder = request.DisplayOrder,
            IsActive = true
        };

        await _repository.AddMenuCategoryAsync(category);
        await _repository.SaveChangesAsync();

        return ServiceResult<MenuCategoryResponse>.Success(new MenuCategoryResponse
        {
            Id = category.Id,
            MenuId = category.MenuId,
            NameSr = category.NameSr,
            NameEn = category.NameEn,
            DescriptionSr = category.DescriptionSr,
            DescriptionEn = category.DescriptionEn,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive
        });
    }

    public async Task<ServiceResult<MenuItemSummaryResponse>> CreateMenuItemAsync(Guid restaurantId, Guid menuId, Guid categoryId, CreateMenuItemRequest request, RequestingUser requestingUser)
    {
        var category = await _repository.GetMenuCategoryByIdAsync(categoryId);

        if (category is null || category.MenuId != menuId)
        {
            return ServiceResult<MenuItemSummaryResponse>.NotFound();
        }

        var menu = await _repository.GetMenuByIdAsync(menuId);

        if (menu is null || menu.RestaurantId != restaurantId)
        {
            return ServiceResult<MenuItemSummaryResponse>.NotFound();
        }

        var restaurant = await _repository.GetByIdAsync(restaurantId);

        if (restaurant is null)
        {
            return ServiceResult<MenuItemSummaryResponse>.NotFound();
        }

        if (!requestingUser.CanManageRestaurant(restaurant.Id, restaurant.OwnerId))
        {
            return ServiceResult<MenuItemSummaryResponse>.Forbidden();
        }

        var item = new MenuItem
        {
            Id = Guid.NewGuid(),
            MenuCategoryId = categoryId,
            RestaurantId = restaurantId,
            NameSr = request.NameSr,
            NameEn = request.NameEn,
            DescriptionSr = request.DescriptionSr,
            DescriptionEn = request.DescriptionEn,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            IsAvailable = request.IsAvailable,
            IsFeatured = request.IsFeatured,
            PreparationTimeMinutes = request.PreparationTimeMinutes
        };

        await _repository.AddMenuItemAsync(item);
        await _repository.SaveChangesAsync();

        return ServiceResult<MenuItemSummaryResponse>.Success(new MenuItemSummaryResponse
        {
            Id = item.Id,
            NameSr = item.NameSr,
            DescriptionSr = item.DescriptionSr,
            Price = item.Price,
            ImageUrl = item.ImageUrl,
            IsAvailable = item.IsAvailable
        });
    }

    public async Task<ServiceResult<MenuResponse>> UpdateMenuAsync(Guid restaurantId, Guid menuId, UpdateMenuRequest request, RequestingUser requestingUser)
    {
        var menu = await _repository.GetMenuByIdAsync(menuId);

        if (menu is null || menu.RestaurantId != restaurantId)
        {
            return ServiceResult<MenuResponse>.NotFound();
        }

        var restaurant = await _repository.GetByIdAsync(restaurantId);

        if (restaurant is null)
        {
            return ServiceResult<MenuResponse>.NotFound();
        }

        if (!requestingUser.CanManageRestaurant(restaurant.Id, restaurant.OwnerId))
        {
            return ServiceResult<MenuResponse>.Forbidden();
        }

        menu.NameSr = request.NameSr;
        menu.NameEn = request.NameEn;
        menu.DescriptionSr = request.DescriptionSr;
        menu.DescriptionEn = request.DescriptionEn;
        menu.DisplayOrder = request.DisplayOrder;
        menu.IsActive = request.IsActive;

        await _repository.SaveChangesAsync();

        return ServiceResult<MenuResponse>.Success(new MenuResponse
        {
            Id = menu.Id,
            RestaurantId = menu.RestaurantId,
            NameSr = menu.NameSr,
            NameEn = menu.NameEn,
            DescriptionSr = menu.DescriptionSr,
            DescriptionEn = menu.DescriptionEn,
            DisplayOrder = menu.DisplayOrder,
            IsActive = menu.IsActive
        });
    }

    public async Task<ServiceResult> DeleteMenuAsync(Guid restaurantId, Guid menuId, RequestingUser requestingUser)
    {
        var menu = await _repository.GetMenuByIdAsync(menuId);

        if (menu is null || menu.RestaurantId != restaurantId)
        {
            return ServiceResult.NotFound();
        }

        var restaurant = await _repository.GetByIdAsync(restaurantId);

        if (restaurant is null)
        {
            return ServiceResult.NotFound();
        }

        if (!requestingUser.CanManageRestaurant(restaurant.Id, restaurant.OwnerId))
        {
            return ServiceResult.Forbidden();
        }

        await _repository.DeleteMenuAsync(menuId);
        await _repository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult<MenuCategoryResponse>> UpdateMenuCategoryAsync(Guid restaurantId, Guid menuId, Guid categoryId, UpdateMenuCategoryRequest request, RequestingUser requestingUser)
    {
        var category = await _repository.GetMenuCategoryByIdAsync(categoryId);

        if (category is null || category.MenuId != menuId)
        {
            return ServiceResult<MenuCategoryResponse>.NotFound();
        }

        var menu = await _repository.GetMenuByIdAsync(menuId);

        if (menu is null || menu.RestaurantId != restaurantId)
        {
            return ServiceResult<MenuCategoryResponse>.NotFound();
        }

        var restaurant = await _repository.GetByIdAsync(restaurantId);

        if (restaurant is null)
        {
            return ServiceResult<MenuCategoryResponse>.NotFound();
        }

        if (!requestingUser.CanManageRestaurant(restaurant.Id, restaurant.OwnerId))
        {
            return ServiceResult<MenuCategoryResponse>.Forbidden();
        }

        category.NameSr = request.NameSr;
        category.NameEn = request.NameEn;
        category.DescriptionSr = request.DescriptionSr;
        category.DescriptionEn = request.DescriptionEn;
        category.DisplayOrder = request.DisplayOrder;
        category.IsActive = request.IsActive;

        await _repository.SaveChangesAsync();

        return ServiceResult<MenuCategoryResponse>.Success(new MenuCategoryResponse
        {
            Id = category.Id,
            MenuId = category.MenuId,
            NameSr = category.NameSr,
            NameEn = category.NameEn,
            DescriptionSr = category.DescriptionSr,
            DescriptionEn = category.DescriptionEn,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive
        });
    }

    public async Task<ServiceResult> DeleteMenuCategoryAsync(Guid restaurantId, Guid menuId, Guid categoryId, RequestingUser requestingUser)
    {
        var category = await _repository.GetMenuCategoryByIdAsync(categoryId);

        if (category is null || category.MenuId != menuId)
        {
            return ServiceResult.NotFound();
        }

        var menu = await _repository.GetMenuByIdAsync(menuId);

        if (menu is null || menu.RestaurantId != restaurantId)
        {
            return ServiceResult.NotFound();
        }

        var restaurant = await _repository.GetByIdAsync(restaurantId);

        if (restaurant is null)
        {
            return ServiceResult.NotFound();
        }

        if (!requestingUser.CanManageRestaurant(restaurant.Id, restaurant.OwnerId))
        {
            return ServiceResult.Forbidden();
        }

        await _repository.DeleteMenuCategoryAsync(categoryId);
        await _repository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<List<HolidayExceptionResponse>?> GetHolidayExceptionsAsync(Guid restaurantId)
    {
        var restaurant = await _repository.GetByIdAsync(restaurantId);

        if (restaurant is null)
        {
            return null;
        }

        var exceptions = await _repository.GetHolidayExceptionsByRestaurantIdAsync(restaurantId);

        return exceptions.Select(MapToHolidayExceptionResponse).ToList();
    }

    public async Task<ServiceResult<HolidayExceptionResponse>> CreateHolidayExceptionAsync(Guid restaurantId, CreateHolidayExceptionRequest request, RequestingUser requestingUser)
    {
        var restaurant = await _repository.GetByIdAsync(restaurantId);

        if (restaurant is null)
        {
            return ServiceResult<HolidayExceptionResponse>.NotFound();
        }

        if (!requestingUser.CanManageRestaurant(restaurant.Id, restaurant.OwnerId))
        {
            return ServiceResult<HolidayExceptionResponse>.Forbidden();
        }

        var exception = new RestaurantHolidayException
        {
            Id = Guid.NewGuid(),
            RestaurantId = restaurantId,
            Date = request.Date,
            IsClosed = request.IsClosed,
            OpenTime = string.IsNullOrWhiteSpace(request.OpenTime) ? null : TimeSpan.Parse(request.OpenTime),
            CloseTime = string.IsNullOrWhiteSpace(request.CloseTime) ? null : TimeSpan.Parse(request.CloseTime),
            Reason = request.Reason
        };

        await _repository.AddHolidayExceptionAsync(exception);
        await _repository.SaveChangesAsync();

        return ServiceResult<HolidayExceptionResponse>.Success(MapToHolidayExceptionResponse(exception));
    }

    public async Task<ServiceResult> DeleteHolidayExceptionAsync(Guid restaurantId, Guid exceptionId, RequestingUser requestingUser)
    {
        var exception = await _repository.GetHolidayExceptionByIdAsync(exceptionId);

        if (exception is null || exception.RestaurantId != restaurantId)
        {
            return ServiceResult.NotFound();
        }

        var restaurant = await _repository.GetByIdAsync(restaurantId);

        if (restaurant is null)
        {
            return ServiceResult.NotFound();
        }

        if (!requestingUser.CanManageRestaurant(restaurant.Id, restaurant.OwnerId))
        {
            return ServiceResult.Forbidden();
        }

        await _repository.DeleteHolidayExceptionAsync(exceptionId);
        await _repository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    private static HolidayExceptionResponse MapToHolidayExceptionResponse(RestaurantHolidayException exception)
    {
        return new HolidayExceptionResponse
        {
            Id = exception.Id,
            RestaurantId = exception.RestaurantId,
            Date = exception.Date,
            IsClosed = exception.IsClosed,
            OpenTime = exception.OpenTime?.ToString(@"hh\:mm"),
            CloseTime = exception.CloseTime?.ToString(@"hh\:mm"),
            Reason = exception.Reason
        };
    }

    private static RestaurantResponse MapToResponse(Restaurant restaurant)
    {
        return new RestaurantResponse
        {
            Id = restaurant.Id,
            NameSr = restaurant.NameSr,
            NameEn = restaurant.NameEn,
            DescriptionSr = restaurant.DescriptionSr,
            DescriptionEn = restaurant.DescriptionEn,
            Address = restaurant.Address,
            ImageUrl = restaurant.ImageUrl,
            IsActive = restaurant.IsActive,
            IsFeatured = restaurant.IsFeatured,
            CuisineType = restaurant.CuisineType,
            IsOpenNow = restaurant.IsOpenNow(DateTime.Now)
        };
    }

    private static MenuSummaryResponse MapToMenuSummary(Menu menu)
    {
        return new MenuSummaryResponse
        {
            MenuId = menu.Id,
            NameSr = menu.NameSr,
            Categories = menu.Categories.Select(MapToCategoryResponse).ToList()
        };
    }

    private static MenuCategoryInMenuResponse MapToCategoryResponse(MenuCategory category)
    {
        return new MenuCategoryInMenuResponse
        {
            Id = category.Id,
            NameSr = category.NameSr,
            DisplayOrder = category.DisplayOrder,
            Items = category.Items.Select(MapToItemSummary).ToList()
        };
    }

    private static MenuItemSummaryResponse MapToItemSummary(MenuItem item)
    {
        return new MenuItemSummaryResponse
        {
            Id = item.Id,
            NameSr = item.NameSr,
            DescriptionSr = item.DescriptionSr,
            Price = item.Price,
            ImageUrl = item.ImageUrl,
            IsAvailable = item.IsAvailable
        };
    }
}
