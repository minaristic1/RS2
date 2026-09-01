using Delivery.Data;
using Delivery.Data.Models;
using MediatR;

namespace Delivery.Api.Features.Deliveries.Commands.AssignCourier
{
    public class AssignCourierCommand : IRequest<DeliveryOrder?>
    {
        public Guid Id { get; set; }
        public Guid CourierId { get; set; }

        public AssignCourierCommand(Guid id, Guid courierId)
        {
            Id = id;
            CourierId = courierId;
        }
    }

    public class AssignCourierCommandHandler : IRequestHandler<AssignCourierCommand, DeliveryOrder?>
    {
        private readonly DeliveryDbContext _context;

        public AssignCourierCommandHandler(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<DeliveryOrder?> Handle(AssignCourierCommand request, CancellationToken cancellationToken)
        {
            var delivery = await _context.Deliveries.FindAsync(new object[] { request.Id }, cancellationToken);

            if (delivery == null)
            {
                return null;
            }

            delivery.CourierId = request.CourierId;
            await _context.SaveChangesAsync(cancellationToken);

            return delivery;
        }
    }
}
