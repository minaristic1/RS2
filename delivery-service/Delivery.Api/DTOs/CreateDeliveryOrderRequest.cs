namespace Delivery.Api.DTOs
{
    public class CreateDeliveryOrderRequest
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public Guid RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string PickupAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CreateOrderItemRequest> Items { get; set; }
    }
}
