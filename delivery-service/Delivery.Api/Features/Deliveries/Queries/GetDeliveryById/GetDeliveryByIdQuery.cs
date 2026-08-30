using Delivery.Data;
using Delivery.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Api.Features.Deliveries.Queries.GetDeliveryById
{
    public class GetDeliveryByIdQuery : IRequest<DeliveryOrder?>
    {
        public Guid Id { get; set; }

        public GetDeliveryByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetDeliveryByIdQueryHandler : IRequestHandler<GetDeliveryByIdQuery, DeliveryOrder?>
    {
        private readonly DeliveryDbContext _context;

        public GetDeliveryByIdQueryHandler(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<DeliveryOrder?> Handle(GetDeliveryByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Deliveries
                .Include(d => d.Items)
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);
        }
    }
}
