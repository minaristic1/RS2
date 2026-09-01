using Billing.Application.Models;
using MediatR;

namespace Billing.Application.Features.Billing.Queries.GetInvoice;

public sealed record GetInvoiceQuery(Guid InvoiceId) : IRequest<InvoiceDto>;

