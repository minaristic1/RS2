using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantService.Domain.Entities
{
    public class Promotion
    {
        public Guid Id { get; set; }

        public string NameSr { get; set; } = string.Empty;

        public string NameEn { get; set; } = string.Empty;

        public decimal DiscountPercentage { get; set; }

        public DateTime StartsAt { get; set; }

        public DateTime EndsAt { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid? MenuItemId { get; set; }
        public Guid? MenuCategoryId { get; set; }
        public Guid? MenuId { get; set; }

        public MenuItem? MenuItem { get; set; }
        public MenuCategory? MenuCategory { get; set; }
        public Menu? Menu { get; set; }
    }
}
