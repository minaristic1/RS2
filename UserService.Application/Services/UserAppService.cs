using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace UserService.Application.Services;

public class UserAppService : IUserAppService
{
    private readonly IUserRepository _repository;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    public UserAppService(IUserRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _passwordHasher = new PasswordHasher<User>();
        _configuration = configuration;
    }

    public async Task<UserResponse?> RegisterAsync(RegisterUserRequest request)
    {
        var existingUser = await _repository.GetByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            return null;
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FullName = request.FullName,
            Role = request.Role,
            IsActive = true,
            RestaurantId = request.RestaurantId,
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            RestaurantId = user.RestaurantId,
        };
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _repository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        var jwtKey = _configuration["Jwt:Key"]!;
        var jwtIssuer = _configuration["Jwt:Issuer"]!;
        var jwtAudience = _configuration["Jwt:Audience"]!;
        var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"]!);

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role.ToString()),
        new Claim("fullName", user.FullName)
    };

        if (user.RestaurantId.HasValue)
        {
            claims.Add(new Claim("restaurantId", user.RestaurantId.Value.ToString()));
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            User = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                RestaurantId = user.RestaurantId
            }
        };
    }
}