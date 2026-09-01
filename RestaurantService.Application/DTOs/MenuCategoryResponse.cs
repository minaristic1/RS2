using System;

namespace RestaurantService.Application.DTOs;

public class MenuCategoryResponse
{
    public Guid Id { get; set; }
    public Guid MenuId { get; set; }
    public string NameSr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string DescriptionSr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}