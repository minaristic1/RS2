using System.ComponentModel.DataAnnotations;

namespace IdentityServer.DTOs;

public sealed record RegisterRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password,
    [Required, StringLength(100)] string FirstName,
    [Required, StringLength(100)] string LastName);

