using Delivery.Data.Models;
using MediatR;

namespace Delivery.Api.Features.Deliveries.Commands.CreateDeliveryOrder
{
    public class CreateDeliveryOrderCommand : IRequest<DeliveryOrder>
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public Guid RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string PickupAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CreateOrderItemDto> Items { get; set; }
    }

    public class CreateOrderItemDto
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
