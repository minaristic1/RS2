namespace Delivery.Api.Messaging.Events
{
    public class OrderReadyForDeliveryEvent
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public Guid RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string PickupAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderReadyForDeliveryItem> Items { get; set; } = new();
    }

    public class OrderReadyForDeliveryItem
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
