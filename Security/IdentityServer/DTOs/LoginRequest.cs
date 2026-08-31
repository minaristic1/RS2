using System.ComponentModel.DataAnnotations;

namespace IdentityServer.DTOs;

public sealed record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);

