using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantService.Application.DTOs;

public class MenuCategoryInMenuResponse
{
    public Guid Id { get; set; }

    public string NameSr { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public List<MenuItemSummaryResponse> Items { get; set; } = new();
}