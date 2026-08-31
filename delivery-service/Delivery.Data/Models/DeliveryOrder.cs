namespace Delivery.Data.Models
{
    public class DeliveryOrder
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public Guid RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string PickupAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal TotalPrice { get; set; }
        public DeliveryStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public Guid? CourierId { get; set; }
        public DateTime? EstimatedDeliveryTime { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        public DeliveryOrder(
            Guid orderId,
            string customerName,
            string customerPhone,
            Guid restaurantId,
            string restaurantName,
            string pickupAddress,
            string deliveryAddress,
            decimal totalPrice)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            CustomerName = customerName;
            CustomerPhone = customerPhone;
            RestaurantId = restaurantId;
            RestaurantName = restaurantName;
            PickupAddress = pickupAddress;
            DeliveryAddress = deliveryAddress;
            TotalPrice = totalPrice;
            Status = DeliveryStatus.Created;
            CreatedAt = DateTime.UtcNow;
        }

        public void AdvanceStatus()
        {
            if (Status == DeliveryStatus.Delivered || Status == DeliveryStatus.Cancelled)
            {
                throw new InvalidOperationException("Ne može se pomeriti status: dostava je već završena ili otkazana.");
            }

            Status = (DeliveryStatus)((int)Status + 1);

            if (Status == DeliveryStatus.Delivered)
            {
                DeliveredAt = DateTime.UtcNow;
            }
        }

        public void Cancel()
        {
            if (Status == DeliveryStatus.Delivered)
            {
                throw new InvalidOperationException("Ne može se otkazati već dostavljena porudžbina.");
            }

            Status = DeliveryStatus.Cancelled;
            CancelledAt = DateTime.UtcNow;
        }
    }
}
