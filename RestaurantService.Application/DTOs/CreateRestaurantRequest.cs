using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using RestaurantService.Domain.ValueObjects;

namespace RestaurantService.Application.DTOs;

public class CreateRestaurantRequest
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
    [StringLength(300)]
    public string Address { get; set; } = string.Empty;

    [Url]
    [StringLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsFeatured { get; set; }

    public CuisineType CuisineType { get; set; }
}