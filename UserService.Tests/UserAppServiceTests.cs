using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

using Moq;

using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;

namespace UserService.Tests;

public class UserAppServiceTests
{
    private static IConfiguration BuildConfiguration()
    {
        var settings = new Dictionary<string, string?>
        {
            { "Jwt:Key", "Test-Tajni-Kljuc-Za-Testove-Minimum32Karaktera" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" },
            { "Jwt:ExpiryMinutes", "60" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailNotTaken_CreatesUserAndReturnsResponse()
    {
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock.Setup(r => r.GetByEmailAsync("novi@test.com")).ReturnsAsync((User?)null);

        var service = new UserAppService(repositoryMock.Object, BuildConfiguration());

        var request = new RegisterUserRequest
        {
            Email = "novi@test.com",
            Password = "lozinka123",
            FullName = "Novi Korisnik",
            Role = UserRole.Customer
        };

        var result = await service.RegisterAsync(request);

        Assert.NotNull(result);
        Assert.Equal("novi@test.com", result!.Email);
        repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ReturnsNull()
    {
        var existing = new User { Id = Guid.NewGuid(), Email = "postoji@test.com" };
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock.Setup(r => r.GetByEmailAsync("postoji@test.com")).ReturnsAsync(existing);

        var service = new UserAppService(repositoryMock.Object, BuildConfiguration());

        var request = new RegisterUserRequest
        {
            Email = "postoji@test.com",
            Password = "lozinka123",
            FullName = "Test",
            Role = UserRole.Customer
        };

        var result = await service.RegisterAsync(request);

        Assert.Null(result);
        repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_HashesPassword_DoesNotStorePlainText()
    {
        User? savedUser = null;
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        repositoryMock.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => savedUser = u)
            .Returns(Task.CompletedTask);

        var service = new UserAppService(repositoryMock.Object, BuildConfiguration());

        var request = new RegisterUserRequest
        {
            Email = "hash@test.com",
            Password = "mojalozinka",
            FullName = "Test",
            Role = UserRole.Customer
        };

        await service.RegisterAsync(request);

        Assert.NotNull(savedUser);
        Assert.NotEqual("mojalozinka", savedUser!.PasswordHash);
    }

    [Fact]
    public async Task RegisterAsync_WithRestaurantOwnerRole_SetsRestaurantId()
    {
        var restaurantId = Guid.NewGuid();
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var service = new UserAppService(repositoryMock.Object, BuildConfiguration());

        var request = new RegisterUserRequest
        {
            Email = "vlasnik@test.com",
            Password = "lozinka123",
            FullName = "Vlasnik",
            Role = UserRole.RestaurantOwner,
            RestaurantId = restaurantId
        };

        var result = await service.RegisterAsync(request);

        Assert.Equal(restaurantId, result!.RestaurantId);
    }

    [Fact]
    public async Task LoginAsync_WithCorrectCredentials_ReturnsTokenAndUser()
    {
        var hasher = new PasswordHasher<User>();
        var user = new User { Id = Guid.NewGuid(), Email = "login@test.com", FullName = "Login Test", Role = UserRole.Customer };
        user.PasswordHash = hasher.HashPassword(user, "tacnalozinka");

        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock.Setup(r => r.GetByEmailAsync("login@test.com")).ReturnsAsync(user);

        var service = new UserAppService(repositoryMock.Object, BuildConfiguration());

        var result = await service.LoginAsync(new LoginRequest { Email = "login@test.com", Password = "tacnalozinka" });

        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result!.Token));
        Assert.Equal(user.Email, result.User.Email);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsNull()
    {
        var hasher = new PasswordHasher<User>();
        var user = new User { Id = Guid.NewGuid(), Email = "login2@test.com", FullName = "Test", Role = UserRole.Customer };
        user.PasswordHash = hasher.HashPassword(user, "tacnalozinka");

        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock.Setup(r => r.GetByEmailAsync("login2@test.com")).ReturnsAsync(user);

        var service = new UserAppService(repositoryMock.Object, BuildConfiguration());

        var result = await service.LoginAsync(new LoginRequest { Email = "login2@test.com", Password = "pogresna" });

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_WithNonexistentEmail_ReturnsNull()
    {
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var service = new UserAppService(repositoryMock.Object, BuildConfiguration());

        var result = await service.LoginAsync(new LoginRequest { Email = "nepostoji@test.com", Password = "bilokoja" });

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_TokenContainsRoleClaim()
    {
        var hasher = new PasswordHasher<User>();
        var user = new User { Id = Guid.NewGuid(), Email = "admin@test.com", FullName = "Admin", Role = UserRole.Admin };
        user.PasswordHash = hasher.HashPassword(user, "adminlozinka");

        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock.Setup(r => r.GetByEmailAsync("admin@test.com")).ReturnsAsync(user);

        var service = new UserAppService(repositoryMock.Object, BuildConfiguration());

        var result = await service.LoginAsync(new LoginRequest { Email = "admin@test.com", Password = "adminlozinka" });

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result!.Token);

        Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
    }
}