namespace IdentityServer.DTOs;

public sealed record AuthResponse(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    UserResponse User);

public sealed record UserResponse(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    IReadOnlyCollection<string> Roles);

