using System.ComponentModel.DataAnnotations;

namespace RestaurantService.Application.DTOs;

public class UpdateMenuCategoryRequest
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

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
