namespace EventBus.Messages.Events;

public sealed class OrderReadyForDeliveryEvent
{
    public Guid OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string PickupAddress { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public List<OrderReadyForDeliveryItem> Items { get; set; } = [];
}

public sealed class OrderReadyForDeliveryItem
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

