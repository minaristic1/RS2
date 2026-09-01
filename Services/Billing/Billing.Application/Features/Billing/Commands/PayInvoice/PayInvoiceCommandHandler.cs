using Billing.Application.Contracts.Persistence;
using Billing.Application.Exceptions;
using Billing.Application.Models;
using Billing.Domain.Exceptions;
using MediatR;

namespace Billing.Application.Features.Billing.Commands.PayInvoice;

public sealed class PayInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
    : IRequestHandler<PayInvoiceCommand, PaymentDto>
{
    public async Task<PaymentDto> Handle(
        PayInvoiceCommand request,
        CancellationToken cancellationToken)
    {
        var invoice = await invoiceRepository.GetDetailsAsync(request.InvoiceId, cancellationToken)
            ?? throw new NotFoundException($"Invoice '{request.InvoiceId}' was not found.");

        if (await invoiceRepository.TransactionReferenceExistsAsync(
                request.TransactionReference,
                cancellationToken))
        {
            throw new BillingDomainException("Transaction reference has already been used.");
        }

        var payment = invoice.RecordPayment(
            request.Method,
            request.Provider,
            request.TransactionReference);

        await invoiceRepository.AddPaymentAsync(invoice, payment, cancellationToken);

        return PaymentDto.FromPayment(payment);
    }
}
