
namespace Delivery.Api.Models
{
    public enum DeliveryStatus
    {
        Created,
        Confirmed,
        Preparing, 
        OutForDelivery,
        Delivered,
        Cancelled
    }
}