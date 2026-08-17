using CartService.Domain1;
using CartService.DTOs;
using CartService.Exceptions;
using CartService.Repositories;
using CartService.Services;
using Moq;

namespace CartService.Tests.Services;

public class CartManagerTests
{
    private readonly Mock<ICartRepository> _repositoryMock;
    private readonly CartManager _cartManager;

    public CartManagerTests()
    {
        _repositoryMock = new Mock<ICartRepository>();
        _cartManager = new CartManager(_repositoryMock.Object);
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
        
        _repositoryMock.Setup(repository => repository.GetCartAsync(userId)).ReturnsAsync((Cart?)null);

        var request = new AddCartItemRequest
        {
            ProductId = productId,
            Quantity = 2
        };

        var result = await _cartManager.AddItemAsync(userId, request);
        
        Assert.Single(result.Items);
        Assert.Equal(productId, result.Items[0].ProductId);
        Assert.Equal(2, result.Items[0].Quantity);
        
        _repositoryMock.Verify(repository => repository.SaveCartAsync(It.IsAny<Cart>()), Times.Once);
    }
    
    [Fact]
    public async Task AddItemAsync_WhenProductAlreadyExists_IncreasesQuantity()
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
                    Quantity = 2
                }
            }
        };
 
        _repositoryMock.Setup(repository => repository.GetCartAsync(userId)).ReturnsAsync(cart);
 
        var request = new AddCartItemRequest
        {
            ProductId = productId,
            Quantity = 3
        };
 
        var result = await _cartManager.AddItemAsync(userId, request);
 
        Assert.Single(result.Items);
        Assert.Equal(5, result.Items[0].Quantity);
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
}