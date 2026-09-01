using Billing.Application.Models;
using Billing.Domain.ValueObjects;
using MediatR;

namespace Billing.Application.Features.Billing.Commands.PayInvoice;

public sealed record PayInvoiceCommand(
    Guid InvoiceId,
    PaymentMethod Method,
    string Provider,
    string TransactionReference,
    string CustomerName,
    string CustomerPhone) : IRequest<PaymentDto>;
