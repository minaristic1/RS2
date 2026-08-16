using System.ComponentModel.DataAnnotations;

namespace CartService.DTOs;

public class AddCartItemRequest
{
    public Guid ProductId { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Količina mora biti veća od nule. ")]
    public int Quantity { get; set; }
}