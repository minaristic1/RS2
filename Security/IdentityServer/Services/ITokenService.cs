using IdentityServer.DTOs;
using IdentityServer.Models;

namespace IdentityServer.Services;

public interface ITokenService
{
    AuthResponse CreateToken(ApplicationUser user, IReadOnlyCollection<string> roles);
}

