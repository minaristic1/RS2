using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Application.Services;

public class UserAppService : IUserAppService
{
    private readonly IUserRepository _repository;
    private readonly PasswordHasher<User> _passwordHasher;

    public UserAppService(IUserRepository repository)
    {
        _repository = repository;
        _passwordHasher = new PasswordHasher<User>();
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
            IsActive = true
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role
        };
    }
}