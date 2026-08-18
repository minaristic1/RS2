using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

using RestaurantService.Application.DTOs;
using RestaurantService.Application.Interfaces;
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

    public async Task<RestaurantResponse> CreateAsync(CreateRestaurantRequest request)
    {
        var restaurant = new Restaurant
        {
            Id = Guid.NewGuid(),
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

    public async Task<bool> UpdateAsync(Guid id, UpdateRestaurantRequest request)
    {
        var restaurant = await _repository.GetByIdAsync(id);

        if (restaurant is null)
        {
            return false;
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

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var restaurant = await _repository.GetByIdAsync(id);

        if (restaurant is null)
        {
            return false;
        }

        await _repository.DeleteAsync(id);
        await _repository.SaveChangesAsync();

        return true;
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
            CuisineType = restaurant.CuisineType
        };
    }
}