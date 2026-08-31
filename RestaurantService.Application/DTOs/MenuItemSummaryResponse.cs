using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantService.Application.DTOs;

public class MenuItemSummaryResponse
{
    public Guid Id { get; set; }

    public string NameSr { get; set; } = string.Empty;

    public string DescriptionSr { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }
}