using Delivery.Data;
using Delivery.Data.Models;
using MediatR;

namespace Delivery.Api.Features.Deliveries.Commands.CreateDeliveryOrder
{
    public class CreateDeliveryOrderCommandHandler : IRequestHandler<CreateDeliveryOrderCommand, DeliveryOrder>
    {
        private readonly DeliveryDbContext _context;

        public CreateDeliveryOrderCommandHandler(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<DeliveryOrder> Handle(CreateDeliveryOrderCommand request, CancellationToken cancellationToken)
        {
            var delivery = new DeliveryOrder(
                request.OrderId, request.CustomerName, request.CustomerPhone,
                request.RestaurantId, request.RestaurantName, request.PickupAddress,
                request.DeliveryAddress, request.TotalPrice
            );

            delivery.Items = request.Items.Select(item => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList();

            _context.Deliveries.Add(delivery);
            await _context.SaveChangesAsync(cancellationToken);

            return delivery;
        }
    }
}
