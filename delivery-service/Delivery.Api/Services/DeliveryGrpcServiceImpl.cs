using Grpc.Core;
using Delivery.Api.Data;
using Delivery.Api.Protos;

namespace Delivery.Api.Services
{
    public class DeliveryGrpcServiceImpl : DeliveryGrpcService.DeliveryGrpcServiceBase
    {
        private readonly DeliveryDbContext _context;

        public DeliveryGrpcServiceImpl(DeliveryDbContext context)
        {
            _context = context;
        }

        public override async Task<DeliveryStatusResponse> GetDeliveryStatus(DeliveryStatusRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.DeliveryId, out var deliveryId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Neispravan format ID-ja."));
            }

            var delivery = await _context.Deliveries.FindAsync(deliveryId);

            if (delivery == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Dostava nije pronađena."));
            }

            return new DeliveryStatusResponse
            {
                DeliveryId = delivery.Id.ToString(),
                Status = (DeliveryStatus)(int)delivery.Status,
                DeliveryAddress = delivery.DeliveryAddress
            };
        }
    }
}
