using Microsoft.AspNetCore.Mvc;

using UserService.Application.DTOs;
using UserService.Application.Interfaces;

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
}