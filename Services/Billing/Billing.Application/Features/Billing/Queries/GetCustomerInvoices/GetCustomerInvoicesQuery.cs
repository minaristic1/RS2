using Billing.Application.Models;
using MediatR;

namespace Billing.Application.Features.Billing.Queries.GetCustomerInvoices;

public sealed record GetCustomerInvoicesQuery(Guid CustomerId)
    : IRequest<IReadOnlyCollection<InvoiceDto>>;

