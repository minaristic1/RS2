using Microsoft.AspNetCore.Mvc;

using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using UserService.Domain.ValueObjects;

namespace UserService.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserAppService _userAppService;

    public UsersController(IUserAppService userAppService)
    {
        _userAppService = userAppService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterUserRequest request)
    {
        if (request.Role != UserRole.Customer && request.Role != UserRole.Driver)
        {
            return BadRequest("Samostalna registracija je dozvoljena samo za role Customer ili Driver. RestaurantOwner i RestaurantEmployee naloge kreira Admin preko /api/users/admin/staff.");
        }

        var user = await _userAppService.RegisterAsync(request);

        if (user is null)
        {
            return Conflict("Korisnik sa ovim email-om vec postoji.");
        }

        return StatusCode(StatusCodes.Status201Created, user);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("admin/staff")]
    public async Task<ActionResult<UserResponse>> CreateStaff([FromBody] RegisterUserRequest request)
    {
        if (request.Role != UserRole.RestaurantOwner && request.Role != UserRole.RestaurantEmployee)
        {
            return BadRequest("Ovaj endpoint sluzi samo za kreiranje RestaurantOwner ili RestaurantEmployee naloga.");
        }

        if (request.Role == UserRole.RestaurantEmployee && request.RestaurantId is null)
        {
            return BadRequest("RestaurantId je obavezan za RestaurantEmployee nalog.");
        }

        var user = await _userAppService.RegisterAsync(request);

        if (user is null)
        {
            return Conflict("Korisnik sa ovim email-om vec postoji.");
        }

        return StatusCode(StatusCodes.Status201Created, user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _userAppService.LoginAsync(request);

        if (result is null)
        {
            return Unauthorized("Pogresan email ili lozinka.");
        }

        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<UserResponse> Me()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        var fullNameClaim = User.FindFirst("fullName")?.Value;
        var restaurantIdClaim = User.FindFirst("restaurantId")?.Value;

        return Ok(new UserResponse
        {
            Id = Guid.Parse(idClaim!),
            Email = emailClaim ?? string.Empty,
            FullName = fullNameClaim ?? string.Empty,
            Role = Enum.Parse<UserRole>(roleClaim!),
            RestaurantId = restaurantIdClaim is null ? null : Guid.Parse(restaurantIdClaim)
        });
    }
}
