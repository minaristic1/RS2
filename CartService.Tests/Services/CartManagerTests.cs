using CartService.Domain1;
using CartService.DTOs;
using CartService.Exceptions;
using CartService.Repositories;
using CartService.Services;
using Moq;
using CartService.Clients;
using CartService.Messaging;
using CartService.Events;

namespace CartService.Tests.Services;

public class CartManagerTests
{
    private readonly Mock<ICartRepository> _repositoryMock;
    private readonly CartManager _cartManager;
    private readonly Mock<IRestaurantClient> _restaurantClientMock;
    private readonly Mock<IRabbitMqPublisher> _rabbitMqPublisherMock;

    public CartManagerTests()
    {
        _repositoryMock = new Mock<ICartRepository>();
        _restaurantClientMock = new Mock<IRestaurantClient>();
        _rabbitMqPublisherMock = new Mock<IRabbitMqPublisher>();
        _cartManager = new CartManager(_repositoryMock.Object, _restaurantClientMock.Object, _rabbitMqPublisherMock.Object);
    }

    [Fact]
    public async Task GetCartAsync_WhenCartDoesNotExist_ReturnsEmptyCart()
    {
        var userId = Guid.NewGuid();
        
        _repositoryMock.Setup(repository => repository.GetCartAsync(userId)).ReturnsAsync((Cart?)null);

        var result = await _cartManager.GetCartAsync(userId);
        
        Assert.Equal(userId, result.UserId);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalPrice);
    }

    [Fact]
    public async Task AddItemAsync_WhenProductDoesNotExist_AddsProduct()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var restaurantId = Guid.NewGuid();
 
        _repositoryMock.Setup(repository => repository.GetCartAsync(userId)).ReturnsAsync((Cart?)null);
 
        _restaurantClientMock.Setup(client => client.GetMenuItemAsync(productId)).ReturnsAsync(new MenuItemResponse
            {
                Id = productId,
                RestaurantId = restaurantId,
                Name = "Capricciosa",
                Price = 850,
                IsAvailable = true
            });
 
        var request = new AddCartItemRequest
        {
            ProductId = productId,
            Quantity = 2
        };
 
        var result = await _cartManager.AddItemAsync(userId, request);
 
        Assert.Single(result.Items);
        Assert.Equal(productId, result.Items[0].ProductId);
        Assert.Equal(restaurantId, result.Items[0].RestaurantId);
        Assert.Equal("Capricciosa", result.Items[0].ProductName);
        Assert.Equal(850, result.Items[0].Price);
        Assert.Equal(2, result.Items[0].Quantity);
 
        _repositoryMock.Verify(
            repository => repository.SaveCartAsync(It.IsAny<Cart>()),
            Times.Once);
    }
    
    [Fact]
    public async Task AddItemAsync_WhenProductAlreadyExists_IncreasesQuantity()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var restaurantId = Guid.NewGuid();
 
        var cart = new Cart
        {
            UserId = userId,
            Items =
            {
                new CartItem
                {
                    ProductId = productId,
                    RestaurantId = restaurantId,
                    ProductName = "Capricciosa",
                    Price = 850,
                    Quantity = 2
                }
            }
        };
 
        _repositoryMock.Setup(repository => repository.GetCartAsync(userId)).ReturnsAsync(cart);
 
        _restaurantClientMock.Setup(client => client.GetMenuItemAsync(productId)).ReturnsAsync(new MenuItemResponse
            {
                Id = productId,
                RestaurantId = restaurantId,
                Name = "Capricciosa",
                Price = 850,
                IsAvailable = true
            });
 
        var request = new AddCartItemRequest
        {
            ProductId = productId,
            Quantity = 3
        };
        
        var result = await _cartManager.AddItemAsync(userId, request);
        
        Assert.Single(result.Items);
 
        Assert.Equal(productId, result.Items[0].ProductId);
 
        Assert.Equal(restaurantId, result.Items[0].RestaurantId);
 
        Assert.Equal("Capricciosa", result.Items[0].ProductName);
 
        Assert.Equal(850, result.Items[0].Price);
 
        Assert.Equal(5, result.Items[0].Quantity);
 
        Assert.Equal(4250, result.TotalPrice);
 
        _repositoryMock.Verify(repository => repository.SaveCartAsync(cart), Times.Once);
 
        _restaurantClientMock.Verify(client => client.GetMenuItemAsync(productId), Times.Once);
    }
 
    [Fact]
    public async Task UpdateItemQuantityAsync_WhenProductDoesNotExist_ThrowsNotFoundException()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
 
        var cart = new Cart
        {
            UserId = userId
        };
 
        _repositoryMock.Setup(repository => repository.GetCartAsync(userId)).ReturnsAsync(cart);
 
        var request = new UpdateCartItemRequest
        {
            Quantity = 5
        };
 
        await Assert.ThrowsAsync<NotFoundException>(() => _cartManager.UpdateItemQuantityAsync(userId, productId, request));
    }
 
    [Fact]
    public async Task RemoveItemAsync_WhenProductExists_RemovesProduct()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
 
        var cart = new Cart
        {
            UserId = userId,
            Items =
            {
                new CartItem
                {
                    ProductId = productId,
                    Quantity = 1
                }
            }
        };
 
        _repositoryMock.Setup(repository => repository.GetCartAsync(userId)).ReturnsAsync(cart);
 
        var result = await _cartManager.RemoveItemAsync(userId, productId);
 
        Assert.Empty(result.Items);
 
        _repositoryMock.Verify(repository => repository.SaveCartAsync(cart), Times.Once);
    }
    
    [Fact]
    public async Task GetCartAsync_WhenCartExists_ReturnsCart()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var cart = new Cart

        {
            UserId = userId,
            Items =
            {
                new CartItem
                {
                    ProductId = productId,
                    Quantity = 2,
                    Price = 100
                }
            }
        };
 
        _repositoryMock.Setup(repository => repository.GetCartAsync(userId)).ReturnsAsync(cart);
 
        var result = await _cartManager.GetCartAsync(userId);
 
        Assert.Equal(userId, result.UserId);
        Assert.Single(result.Items);
        Assert.Equal(productId, result.Items[0].ProductId);
        Assert.Equal(2, result.Items[0].Quantity);
        Assert.Equal(200, result.TotalPrice);

    }
    
    [Fact]
    public async Task UpdateItemQuantityAsync_WhenProductExists_UpdatesQuantity()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var cart = new Cart
            
        {
            UserId = userId,
            Items =
            {
                new CartItem
                {
                    ProductId = productId,
                    Quantity = 1
                }
            }
        };
 
        _repositoryMock.Setup(repository => repository.GetCartAsync(userId)).ReturnsAsync(cart);
 
        var request = new UpdateCartItemRequest
        {
            Quantity = 4
        };
 
        var result = await _cartManager.UpdateItemQuantityAsync(userId, productId, request);
 
        Assert.Equal(4, result.Items[0].Quantity);
 
        _repositoryMock.Verify(repository => repository.SaveCartAsync(cart), Times.Once);
    }
    
    [Fact]
    public async Task ClearCartAsync_DeletesCart()
    {
        var userId = Guid.NewGuid();
 
        await _cartManager.ClearCartAsync(userId);
 
        _repositoryMock.Verify(repository => repository.DeleteCartAsync(userId), Times.Once);
    }
    
    [Fact]
    public async Task AddItemAsync_WhenProductDoesNotExist_ThrowsNotFoundException()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
 
        _restaurantClientMock.Setup(client => client.GetMenuItemAsync(productId)).ReturnsAsync((MenuItemResponse?)null);
 
        var request = new AddCartItemRequest
        {
            ProductId = productId,
            Quantity = 1
        };
 
        await Assert.ThrowsAsync<NotFoundException>(() => _cartManager.AddItemAsync(userId, request));
 
        _repositoryMock.Verify(repository => repository.SaveCartAsync(It.IsAny<Cart>()), Times.Never);
    }
    
    [Fact]
    public async Task AddItemAsync_WhenProductIsUnavailable_ThrowsConflictException()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
 
        _restaurantClientMock.Setup(client => client.GetMenuItemAsync(productId)).ReturnsAsync(new MenuItemResponse
            {
                Id = productId,
                RestaurantId = Guid.NewGuid(),
                Name = "Capricciosa",
                Price = 850,
                IsAvailable = false
            });
 
        var request = new AddCartItemRequest
        {
            ProductId = productId,
            Quantity = 1
        };
 
        await Assert.ThrowsAsync<ConflictException>(() => _cartManager.AddItemAsync(userId, request));
 
        _repositoryMock.Verify(repository => repository.SaveCartAsync(It.IsAny<Cart>()), Times.Never);
    }
    
    [Fact]
    public async Task AddItemAsync_WhenProductIsFromDifferentRestaurant_ThrowsConflictException()
    {
        var userId = Guid.NewGuid();
     
        var existingProductId = Guid.NewGuid();
        var newProductId = Guid.NewGuid();
     
        var existingRestaurantId = Guid.NewGuid();
        var differentRestaurantId = Guid.NewGuid();
     
        var cart = new Cart
        {
            UserId = userId,
            Items =
            {
                new CartItem
                {
                    ProductId = existingProductId,
                    RestaurantId = existingRestaurantId,
                    ProductName = "Pizza",
                    Price = 800,
                    Quantity = 1
                }
            }
        };
     
        _repositoryMock.Setup(repository => repository.GetCartAsync(userId)).ReturnsAsync(cart);
     
        _restaurantClientMock.Setup(client => client.GetMenuItemAsync(newProductId)).ReturnsAsync(new MenuItemResponse
            {
                Id = newProductId,
                RestaurantId = differentRestaurantId,
                Name = "Burger",
                Price = 700,
                IsAvailable = true
            });
     
        var request = new AddCartItemRequest
        {
            ProductId = newProductId,
            Quantity = 1
        };
     
        await Assert.ThrowsAsync<ConflictException>(() => _cartManager.AddItemAsync(userId, request));
     
        _repositoryMock.Verify(repository => repository.SaveCartAsync(It.IsAny<Cart>()), Times.Never);
    }
    
    [Fact]
    public async Task CheckoutAsync_WhenCartDoesNotExist_ThrowsNotFoundException()
    {
        var userId = Guid.NewGuid();

        _repositoryMock
            .Setup(repository => repository.GetCartAsync(userId))
            .ReturnsAsync((Cart?)null);

        var request = new CheckoutRequest { DeliveryAddress = "Ulica Slobode 5" };

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _cartManager.CheckoutAsync(userId, request));
 
        _rabbitMqPublisherMock.Verify(
            publisher => publisher.PublishCartCheckedOutAsync(
                It.IsAny<CartCheckedOutEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
 
        _repositoryMock.Verify(
            repository => repository.DeleteCartAsync(userId),
            Times.Never);
    }
    
    [Fact]
    public async Task CheckoutAsync_WhenCartIsEmpty_ThrowsNotFoundException()
    {
        var userId = Guid.NewGuid();
 
        var cart = new Cart
        {
            UserId = userId
        };
 
        _repositoryMock
            .Setup(repository => repository.GetCartAsync(userId))
            .ReturnsAsync(cart);

        var request = new CheckoutRequest { DeliveryAddress = "Ulica Slobode 5" };

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _cartManager.CheckoutAsync(userId, request));
 
        _rabbitMqPublisherMock.Verify(
            publisher => publisher.PublishCartCheckedOutAsync(
                It.IsAny<CartCheckedOutEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
 
        _repositoryMock.Verify(
            repository => repository.DeleteCartAsync(userId),
            Times.Never);
    }
    
    [Fact]
    public async Task CheckoutAsync_WhenCartHasItems_PublishesEventAndDeletesCart()
    {
        var userId = Guid.NewGuid();
        var restaurantId = Guid.NewGuid();
        var productId = Guid.NewGuid();
 
        var cart = new Cart
        {
            UserId = userId,
            Items =
            {
                new CartItem
                {
                    ProductId = productId,
                    RestaurantId = restaurantId,
                    ProductName = "Capricciosa",
                    Price = 850,
                    Quantity = 2
                }
            }
        };
 
        _repositoryMock
            .Setup(repository => repository.GetCartAsync(userId))
            .ReturnsAsync(cart);

        var request = new CheckoutRequest { DeliveryAddress = "Ulica Slobode 5" };

        await _cartManager.CheckoutAsync(userId, request);

        _rabbitMqPublisherMock.Verify(
            publisher => publisher.PublishCartCheckedOutAsync(
                It.Is<CartCheckedOutEvent>(message =>
                    message.UserId == userId &&
                    message.RestaurantId == restaurantId &&
                    message.DeliveryAddress == "Ulica Slobode 5" &&
                    message.TotalPrice == 1700 &&
                    message.Items.Count == 1 &&
                    message.Items[0].ProductId == productId &&
                    message.Items[0].ProductName == "Capricciosa" &&
                    message.Items[0].Price == 850 &&
                    message.Items[0].Quantity == 2),
                It.IsAny<CancellationToken>()),
            Times.Once);
 
        _repositoryMock.Verify(
            repository => repository.DeleteCartAsync(userId),
            Times.Once);
    }
}