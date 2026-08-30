using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantService.Domain.Entities
{
    public class RestaurantOpeningHours
    {
        public Guid Id { get; set; }

        public Guid RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; } = null!;

        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }

        public bool IsClosed { get; set; } = false;
    }
}