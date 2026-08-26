using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantService.Application.DTOs;

public class MenuSummaryResponse
{
    public Guid MenuId { get; set; }

    public string NameSr { get; set; } = string.Empty;

    public List<MenuCategoryInMenuResponse> Categories { get; set; } = new();
}