namespace Delivery.Api.Models
{
    public class DeliveryOrder
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string DeliveryAddress { get; set; }
        public DeliveryStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        public DeliveryOrder(Guid orderId, string deliveryAddress)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            DeliveryAddress = deliveryAddress;
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
