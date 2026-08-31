namespace CartService.DTOs;

public class CartResponse
{
    public Guid UserId { get; set; }
    
    public List<CartItemResponse> Items { get; set; } = new();
    
    public decimal TotalPrice { get; set; }
}