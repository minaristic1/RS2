using Billing.Domain.Aggregates;

namespace Billing.Application.Models;

public sealed record InvoiceDto(
    Guid Id,
    Guid OrderId,
    Guid CustomerId,
    Guid RestaurantId,
    string DeliveryAddress,
    string Currency,
    decimal TotalAmount,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PaidAt,
    IReadOnlyCollection<InvoiceItemDto> Items,
    IReadOnlyCollection<PaymentDto> Payments)
{
    public static InvoiceDto FromInvoice(Invoice invoice)
    {
        return new InvoiceDto(
            invoice.Id,
            invoice.OrderId,
            invoice.CustomerId,
            invoice.RestaurantId,
            invoice.DeliveryAddress,
            invoice.Currency,
            invoice.TotalAmount,
            invoice.Status.ToString(),
            invoice.CreatedAt,
            invoice.PaidAt,
            invoice.Items
                .Select(item => new InvoiceItemDto(
                    item.ProductId,
                    item.Name,
                    item.Quantity,
                    item.UnitPrice,
                    item.Total))
                .ToArray(),
            invoice.Payments.Select(PaymentDto.FromPayment).ToArray());
    }
}

public sealed record InvoiceItemDto(
    Guid ProductId,
    string Name,
    int Quantity,
    decimal UnitPrice,
    decimal Total);
