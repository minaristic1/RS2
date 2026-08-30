using Delivery.Data;
using Delivery.Data.Models;
using MediatR;

namespace Delivery.Api.Features.Deliveries.Commands.CancelDelivery
{
    public class CancelDeliveryCommand : IRequest<DeliveryOrder?>
    {
        public Guid Id { get; set; }

        public CancelDeliveryCommand(Guid id)
        {
            Id = id;
        }
    }

    public class CancelDeliveryCommandHandler : IRequestHandler<CancelDeliveryCommand, DeliveryOrder?>
    {
        private readonly DeliveryDbContext _context;

        public CancelDeliveryCommandHandler(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<DeliveryOrder?> Handle(CancelDeliveryCommand request, CancellationToken cancellationToken)
        {
            var delivery = await _context.Deliveries.FindAsync(new object[] { request.Id }, cancellationToken);

            if (delivery == null)
            {
                return null;
            }

            delivery.Cancel();
            await _context.SaveChangesAsync(cancellationToken);

            return delivery;
        }
    }
}
