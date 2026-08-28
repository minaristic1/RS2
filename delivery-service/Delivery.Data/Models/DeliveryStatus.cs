
namespace Delivery.Data.Models
{
    public enum DeliveryStatus
    {
        Created,
        Confirmed,
        Preparing,
        ReadyForPickup,
        OutForDelivery,
        Delivered,
        Cancelled
    }
}