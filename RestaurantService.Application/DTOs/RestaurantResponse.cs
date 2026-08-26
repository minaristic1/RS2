using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantService.Application.DTOs;

public class RestaurantResponse
{
    public Guid Id { get; set; }

    public string NameSr { get; set; } = string.Empty;

    public string NameEn { get; set; } = string.Empty;

    public string DescriptionSr { get; set; } = string.Empty;

    public string DescriptionEn { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public bool IsFeatured { get; set; }

    public string CuisineType { get; set; } = string.Empty;

    public bool IsOpenNow { get; set; }
}