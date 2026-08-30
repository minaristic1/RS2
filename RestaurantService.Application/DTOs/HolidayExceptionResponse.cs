namespace RestaurantService.Application.DTOs;

public class HolidayExceptionResponse
{
    public Guid Id { get; set; }

    public Guid RestaurantId { get; set; }

    public DateOnly Date { get; set; }

    public bool IsClosed { get; set; }

    public string? OpenTime { get; set; }

    public string? CloseTime { get; set; }

    public string? Reason { get; set; }
}
