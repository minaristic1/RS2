using Delivery.Data;
using Delivery.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Api.Features.Deliveries.Queries.GetDeliveryByOrderId
{
    public class GetDeliveryByOrderIdQuery : IRequest<DeliveryOrder?>
    {
        public Guid OrderId { get; set; }

        public GetDeliveryByOrderIdQuery(Guid orderId)
        {
            OrderId = orderId;
        }
    }

    public class GetDeliveryByOrderIdQueryHandler : IRequestHandler<GetDeliveryByOrderIdQuery, DeliveryOrder?>
    {
        private readonly DeliveryDbContext _context;

        public GetDeliveryByOrderIdQueryHandler(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<DeliveryOrder?> Handle(GetDeliveryByOrderIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Deliveries
                .Include(d => d.Items)
                .FirstOrDefaultAsync(d => d.OrderId == request.OrderId, cancellationToken);
        }
    }
}
