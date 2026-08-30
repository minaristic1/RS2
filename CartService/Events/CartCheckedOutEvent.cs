namespace CartService.Events;

public class CartCheckedOutEvent
{
    public Guid UserId { get; set; }
 
    public Guid RestaurantId { get; set; }

    public string DeliveryAddress { get; set; } = string.Empty;

    public List<CartCheckedOutItem> Items { get; set; } = new();
 
    public decimal TotalPrice { get; set; }
 
    public DateTime CreatedAt { get; set; }
}
 
public class CartCheckedOutItem
{
    public Guid ProductId { get; set; }
 
    public string ProductName { get; set; } = string.Empty;
 
    public decimal Price { get; set; }
 
    public int Quantity { get; set; }
}