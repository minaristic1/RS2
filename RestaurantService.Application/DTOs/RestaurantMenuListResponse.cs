using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantService.Application.DTOs;

public class RestaurantMenuListResponse
{
    public Guid RestaurantId { get; set; }

    public List<MenuSummaryResponse> Menus { get; set; } = new();
}