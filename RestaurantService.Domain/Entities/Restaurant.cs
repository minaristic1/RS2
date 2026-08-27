using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using RestaurantService.Domain.ValueObjects;

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

        public CuisineType CuisineType { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsFeatured { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Menu> Menus { get; set; } = new();

        public List<RestaurantOpeningHours> OpeningHours { get; set; } = new();

        public List<RestaurantHolidayException> HolidayExceptions { get; set; } = new();

        public bool IsOpenNow(DateTime referenceTime)
        {
            var date = DateOnly.FromDateTime(referenceTime);
            var holidayException = HolidayExceptions.FirstOrDefault(exception => exception.Date == date);

            if (holidayException is not null)
            {
                if (holidayException.IsClosed || holidayException.OpenTime is null || holidayException.CloseTime is null)
                {
                    return false;
                }

                var currentTimeOnHoliday = referenceTime.TimeOfDay;
                return currentTimeOnHoliday >= holidayException.OpenTime && currentTimeOnHoliday < holidayException.CloseTime;
            }

            var todayHours = OpeningHours.FirstOrDefault(hours => hours.DayOfWeek == referenceTime.DayOfWeek);

            if (todayHours is null || todayHours.IsClosed)
            {
                return false;
            }

            var currentTime = referenceTime.TimeOfDay;
            return currentTime >= todayHours.OpenTime && currentTime < todayHours.CloseTime;
        }
    }
}
