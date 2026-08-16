using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantService.Domain.Entities
{
    public class MenuItem
    {
        public Guid Id { get; set; }

        public Guid MenuCategoryId { get; set; }

        public string NameSr { get; set; } = string.Empty;

        public string NameEn { get; set; } = string.Empty;

        public string DescriptionSr { get; set; } = string.Empty;

        public string DescriptionEn { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        public bool IsFeatured { get; set; }

        public int PreparationTimeMinutes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public MenuCategory MenuCategory { get; set; } = null!;

        public List<Promotion> Promotions { get; set; } = new();
    }
}
