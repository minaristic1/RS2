using Moq;
using RestaurantService.Application.Interfaces;
using RestaurantService.Application.DTOs;
using RestaurantService.Application.Services;
using RestaurantService.Domain.Entities;

namespace RestaurantService.Tests.Services;

public class RestaurantAppServiceTests
{
    private readonly Mock<IRestaurantRepository> _repositoryMock;
    private readonly RestaurantAppService _appService;

    public RestaurantAppServiceTests()
    {
        _repositoryMock = new Mock<IRestaurantRepository>();
        _appService = new RestaurantAppService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRestaurantDoesNotExist_ReturnsNull()
    {
        var restaurantId = Guid.NewGuid();

        _repositoryMock.Setup(repository => repository.GetByIdAsync(restaurantId)).ReturnsAsync((Restaurant?)null);

        var result = await _appService.GetByIdAsync(restaurantId);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenRestaurantDoesNotExist_ReturnsFalse()
    {
        var restaurantId = Guid.NewGuid();

        _repositoryMock.Setup(repository => repository.GetByIdAsync(restaurantId)).ReturnsAsync((Restaurant?)null);

        var result = await _appService.DeleteAsync(restaurantId);

        Assert.False(result);

        _repositoryMock.Verify(repository => repository.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenRestaurantExists_DeletesAndReturnsTrue()
    {
        var restaurantId = Guid.NewGuid();
        var restaurant = new Restaurant { Id = restaurantId };

        _repositoryMock.Setup(repository => repository.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

        var result = await _appService.DeleteAsync(restaurantId);

        Assert.True(result);

        _repositoryMock.Verify(repository => repository.DeleteAsync(restaurantId), Times.Once);
        _repositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetRestaurantMenuAsync_WhenRestaurantDoesNotExist_ReturnsNull()
    {
        var restaurantId = Guid.NewGuid();

        _repositoryMock.Setup(repository => repository.GetByIdAsync(restaurantId)).ReturnsAsync((Restaurant?)null);

        var result = await _appService.GetRestaurantMenuAsync(restaurantId);

        Assert.Null(result);

        _repositoryMock.Verify(repository => repository.GetMenusByRestaurantIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetRestaurantMenuAsync_WhenRestaurantExists_ReturnsMappedMenu()
    {
        var restaurantId = Guid.NewGuid();
        var restaurant = new Restaurant { Id = restaurantId };

        var menuItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            NameSr = "Pica Margarita",
            DescriptionSr = "Paradajz, mocarela, bosiljak",
            Price = 650,
            ImageUrl = "https://example.com/margarita.jpg",
            IsAvailable = true
        };

        var category = new MenuCategory
        {
            Id = Guid.NewGuid(),
            NameSr = "Pice",
            DisplayOrder = 1,
            Items = new List<MenuItem> { menuItem }
        };

        var menu = new Menu
        {
            Id = Guid.NewGuid(),
            NameSr = "Glavni meni",
            Categories = new List<MenuCategory> { category }
        };

        _repositoryMock.Setup(repository => repository.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);
        _repositoryMock.Setup(repository => repository.GetMenusByRestaurantIdAsync(restaurantId)).ReturnsAsync(new List<Menu> { menu });

        var result = await _appService.GetRestaurantMenuAsync(restaurantId);

        Assert.NotNull(result);
        Assert.Equal(restaurantId, result.RestaurantId);
        Assert.Single(result.Menus);
        Assert.Equal("Glavni meni", result.Menus[0].NameSr);
        Assert.Single(result.Menus[0].Categories);
        Assert.Equal("Pice", result.Menus[0].Categories[0].NameSr);
        Assert.Single(result.Menus[0].Categories[0].Items);
        Assert.Equal("Pica Margarita", result.Menus[0].Categories[0].Items[0].NameSr);
    }

    [Fact]
    public async Task UpdateAsync_WhenRestaurantDoesNotExist_ReturnsFalse()
    {
        var restaurantId = Guid.NewGuid();
        var request = new UpdateRestaurantRequest();

        _repositoryMock.Setup(repository => repository.GetByIdAsync(restaurantId)).ReturnsAsync((Restaurant?)null);

        var result = await _appService.UpdateAsync(restaurantId, request);

        Assert.False(result);

        _repositoryMock.Verify(repository => repository.UpdateAsync(It.IsAny<Restaurant>()), Times.Never);
    }
}