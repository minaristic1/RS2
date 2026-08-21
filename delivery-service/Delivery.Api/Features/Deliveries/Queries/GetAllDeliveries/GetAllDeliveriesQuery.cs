using Delivery.Data;
using Delivery.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Api.Features.Deliveries.Queries.GetAllDeliveries
{
    public class GetAllDeliveriesQuery : IRequest<List<DeliveryOrder>>
    {
        public DeliveryStatus? Status { get; set; }

        public GetAllDeliveriesQuery(DeliveryStatus? status)
        {
            Status = status;
        }
    }

    public class GetAllDeliveriesQueryHandler : IRequestHandler<GetAllDeliveriesQuery, List<DeliveryOrder>>
    {
        private readonly DeliveryDbContext _context;

        public GetAllDeliveriesQueryHandler(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<List<DeliveryOrder>> Handle(GetAllDeliveriesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Deliveries.Include(d => d.Items).AsQueryable();

            if (request.Status.HasValue)
            {
                query = query.Where(d => d.Status == request.Status.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }
    }
}
