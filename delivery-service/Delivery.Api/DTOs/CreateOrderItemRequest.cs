namespace Delivery.Api.DTOs
{
    public class CreateOrderItemRequest
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
