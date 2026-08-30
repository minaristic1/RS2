using System;

namespace RestaurantService.Application.DTOs;

public class MenuResponse
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public string NameSr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string DescriptionSr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}