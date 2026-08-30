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