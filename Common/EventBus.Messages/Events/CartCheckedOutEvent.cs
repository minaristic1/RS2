namespace EventBus.Messages.Events;

public sealed class CartCheckedOutEvent
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid RestaurantId { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public List<CartCheckedOutItem> Items { get; set; } = [];
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class CartCheckedOutItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

