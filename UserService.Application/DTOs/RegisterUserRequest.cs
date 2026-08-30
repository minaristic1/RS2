using System.ComponentModel.DataAnnotations;

using UserService.Domain.ValueObjects;

namespace UserService.Application.DTOs
{
    public class RegisterUserRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        public Guid? RestaurantId { get; set; }
    }
}