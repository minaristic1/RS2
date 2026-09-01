namespace CartService.DTOs;

public class CartItemResponse
{
    public Guid ProductId { get; set; }
    
    public Guid RestaurantId { get; set; }
    
    public string ProductName { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
    
    public int Quantity { get; set; }

    public decimal TotalPrice => Price * Quantity;
}