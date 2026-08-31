namespace CartService.Domain1;

public class Cart
{
    public Guid UserId { get; set; }

    public Guid? CheckoutOrderId { get; set; }

    public List<CartItem> Items { get; set; } = new();
    
    public decimal TotalPrice => Items.Sum(item => item.Price * item.Quantity);
}