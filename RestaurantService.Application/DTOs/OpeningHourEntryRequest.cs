namespace RestaurantService.Application.DTOs;

public class OpeningHourEntryRequest
{
    public DayOfWeek DayOfWeek { get; set; }
    public string OpenTime { get; set; } = "00:00";
    public string CloseTime { get; set; } = "00:00";
    public bool IsClosed { get; set; }
}