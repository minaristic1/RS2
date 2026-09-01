using Billing.Application.Models;
using MediatR;

namespace Billing.Application.Features.Billing.Commands.CreateInvoice;

public sealed record CreateInvoiceCommand(
    Guid OrderId,
    Guid CustomerId,
    string Currency,
    IReadOnlyCollection<CreateInvoiceItem> Items) : IRequest<InvoiceDto>;

public sealed record CreateInvoiceItem(
    Guid ProductId,
    string Name,
    int Quantity,
    decimal UnitPrice);

