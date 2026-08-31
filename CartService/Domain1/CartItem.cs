namespace CartService.Domain1;

public class CartItem
{
    public Guid ProductId { get; set; }
    
    public Guid RestaurantId { get; set; }
    
    public string ProductName { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
    
    public int Quantity { get; set; }
}