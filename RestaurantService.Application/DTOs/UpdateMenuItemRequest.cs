using System.ComponentModel.DataAnnotations;

namespace RestaurantService.Application.DTOs;

public class UpdateMenuItemRequest
{
    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string NameSr { get; set; } = string.Empty;

    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string NameEn { get; set; } = string.Empty;

    [StringLength(1000)]
    public string DescriptionSr { get; set; } = string.Empty;

    [StringLength(1000)]
    public string DescriptionEn { get; set; } = string.Empty;

    [Required]
    [Range(0, 100000)]
    public decimal Price { get; set; }

    [Url]
    [StringLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;
    public bool IsFeatured { get; set; }
    public int PreparationTimeMinutes { get; set; }
}