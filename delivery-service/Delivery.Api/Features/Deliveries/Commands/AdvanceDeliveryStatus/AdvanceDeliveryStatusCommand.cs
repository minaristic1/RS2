using Delivery.Data;
using Delivery.Data.Models;
using MediatR;

namespace Delivery.Api.Features.Deliveries.Commands.AdvanceDeliveryStatus
{
    public class AdvanceDeliveryStatusCommand : IRequest<DeliveryOrder?>
    {
        public Guid Id { get; set; }

        public AdvanceDeliveryStatusCommand(Guid id)
        {
            Id = id;
        }
    }

    public class AdvanceDeliveryStatusCommandHandler : IRequestHandler<AdvanceDeliveryStatusCommand, DeliveryOrder?>
    {
        private readonly DeliveryDbContext _context;

        public AdvanceDeliveryStatusCommandHandler(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<DeliveryOrder?> Handle(AdvanceDeliveryStatusCommand request, CancellationToken cancellationToken)
        {
            var delivery = await _context.Deliveries.FindAsync(new object[] { request.Id }, cancellationToken);

            if (delivery == null)
            {
                return null;
            }

            delivery.AdvanceStatus();
            await _context.SaveChangesAsync(cancellationToken);

            return delivery;
        }
    }
}
