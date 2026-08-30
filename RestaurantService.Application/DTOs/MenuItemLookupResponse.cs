using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantService.Application.DTOs;

public class MenuItemLookupResponse
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
}