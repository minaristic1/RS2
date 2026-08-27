using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantService.Domain.Entities
{
    public class Menu
    {
        public Guid Id { get; set; }

        public Guid RestaurantId { get; set; }

        public string NameSr { get; set; } = string.Empty;

        public string NameEn { get; set; } = string.Empty;

        public string DescriptionSr { get; set; } = string.Empty;

        public string DescriptionEn { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; }

        public Restaurant Restaurant { get; set; } = null!;

        public List<MenuCategory> Categories { get; set; } = new();

        public List<Promotion> Promotions { get; set; } = new();
    }
}
