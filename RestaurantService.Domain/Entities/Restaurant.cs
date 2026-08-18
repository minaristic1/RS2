using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantService.Domain.Entities
{
    public class Restaurant
    {
        public Guid Id { get; set; }

        public string NameSr { get; set; } = string.Empty;

        public string NameEn { get; set; } = string.Empty;

        public string DescriptionSr { get; set; } = string.Empty;

        public string DescriptionEn { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public string CuisineType { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public bool IsFeatured { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Menu> Menus { get; set; } = new();

        public List<RestaurantOpeningHours> OpeningHours { get; set; } = new();
    }
}
