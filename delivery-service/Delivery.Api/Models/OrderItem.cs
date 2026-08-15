namespace Delivery.Api.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Guid DeliveryOrderId { get; set; }
    }
}
