using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantService.Domain.Entities
{
    public class RestaurantHolidayException
    {
        public Guid Id { get; set; }

        public Guid RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; } = null!;

        public DateOnly Date { get; set; }

        public bool IsClosed { get; set; } = true;

        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }

        public string? Reason { get; set; }
    }
}