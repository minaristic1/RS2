using System.ComponentModel.DataAnnotations;

namespace CartService.DTOs;

public class CheckoutRequest
{
    [Required(ErrorMessage = "Adresa dostave je obavezna.")]
    public string DeliveryAddress { get; set; } = string.Empty;
}
