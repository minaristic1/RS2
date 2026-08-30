using System.ComponentModel.DataAnnotations;

namespace RestaurantService.Application.DTOs;

public class CreateHolidayExceptionRequest
{
    [Required]
    public DateOnly Date { get; set; }

    public bool IsClosed { get; set; } = true;

    public string? OpenTime { get; set; }

    public string? CloseTime { get; set; }

    [StringLength(300)]
    public string? Reason { get; set; }
}
