using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace RestaurantService.Domain.Entities
{
    public class MenuCategory
    {
        public Guid Id { get; set; }

        public Guid MenuId { get; set; }

        public string NameSr { get; set; } = string.Empty;

        public string NameEn { get; set; } = string.Empty;

        public string DescriptionSr { get; set; } = string.Empty;

        public string DescriptionEn { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public Menu Menu { get; set; } = null!;

        public List<MenuItem> Items { get; set; } = new();

        public List<Promotion> Promotions { get; set; } = new();
    }
}
