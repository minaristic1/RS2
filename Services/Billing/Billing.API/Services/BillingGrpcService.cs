using Billing.Application.Exceptions;
using Billing.Application.Features.Billing.Commands.CreateInvoice;
using Billing.Application.Features.Billing.Queries.GetInvoice;
using Billing.Application.Models;
using Billing.Domain.Exceptions;
using Grpc.Core;
using MediatR;
using GrpcContract = Billing.API.Grpc.BillingService;

namespace Billing.API.Services;

public sealed class BillingGrpcService(ISender sender) : GrpcContract.BillingServiceBase
{
    public override async Task<Grpc.InvoiceGrpcResponse> CreateInvoice(
        Grpc.CreateInvoiceGrpcRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.OrderId, out var orderId) ||
            !Guid.TryParse(request.CustomerId, out var customerId) ||
            !Guid.TryParse(request.RestaurantId, out var restaurantId))
        {
            throw new RpcException(new Status(
                StatusCode.InvalidArgument,
                "Order and customer identifiers must be valid UUID values."));
        }

        var items = request.Items.Select(item =>
        {
            if (!Guid.TryParse(item.ProductId, out var productId))
            {
                throw new RpcException(new Status(
                    StatusCode.InvalidArgument,
                    "Every product identifier must be a valid UUID value."));
            }

            return new CreateInvoiceItem(
                productId,
                item.Name,
                item.Quantity,
                Convert.ToDecimal(item.UnitPrice));
        }).ToArray();

        try
        {
            var invoice = await sender.Send(
                new CreateInvoiceCommand(
                    orderId,
                    customerId,
                    restaurantId,
                    request.DeliveryAddress,
                    request.Currency,
                    items),
                context.CancellationToken);

            return ToGrpcResponse(invoice);
        }
        catch (BillingDomainException exception)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, exception.Message));
        }
    }

    public override async Task<Grpc.InvoiceGrpcResponse> GetInvoice(
        Grpc.GetInvoiceGrpcRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.InvoiceId, out var invoiceId))
        {
            throw new RpcException(new Status(
                StatusCode.InvalidArgument,
                "Invoice identifier must be a valid UUID value."));
        }

        try
        {
            var invoice = await sender.Send(
                new GetInvoiceQuery(invoiceId),
                context.CancellationToken);

            return ToGrpcResponse(invoice);
        }
        catch (NotFoundException exception)
        {
            throw new RpcException(new Status(StatusCode.NotFound, exception.Message));
        }
    }

    private static Grpc.InvoiceGrpcResponse ToGrpcResponse(InvoiceDto invoice)
    {
        return new Grpc.InvoiceGrpcResponse
        {
            Id = invoice.Id.ToString(),
            OrderId = invoice.OrderId.ToString(),
            CustomerId = invoice.CustomerId.ToString(),
            RestaurantId = invoice.RestaurantId.ToString(),
            DeliveryAddress = invoice.DeliveryAddress,
            Currency = invoice.Currency,
            TotalAmount = Convert.ToDouble(invoice.TotalAmount),
            Status = invoice.Status
        };
    }
}
