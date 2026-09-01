using Billing.Application.Contracts.Infrastructure;
using Billing.Application.Contracts.Persistence;
using Billing.Application.Exceptions;
using Billing.Application.Models;
using Billing.Domain.Exceptions;
using MediatR;

namespace Billing.Application.Features.Billing.Commands.PayInvoice;

public sealed class PayInvoiceCommandHandler(
    IInvoiceRepository invoiceRepository,
    IRestaurantService restaurantService,
    IOrderReadyForDeliveryPublisher deliveryPublisher)
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

        if (invoice.RestaurantId == Guid.Empty ||
            string.IsNullOrWhiteSpace(invoice.DeliveryAddress))
        {
            throw new BillingDomainException(
                "Invoice does not contain delivery information.");
        }

        var restaurant = await restaurantService.GetRestaurantAsync(
                invoice.RestaurantId,
                cancellationToken)
            ?? throw new NotFoundException(
                $"Restaurant '{invoice.RestaurantId}' was not found.");

        var payment = invoice.RecordPayment(
            request.Method,
            request.Provider,
            request.TransactionReference);

        await invoiceRepository.AddPaymentAsync(invoice, payment, cancellationToken);
        await deliveryPublisher.PublishAsync(
            invoice,
            restaurant,
            request.CustomerName,
            request.CustomerPhone,
            cancellationToken);

        return PaymentDto.FromPayment(payment);
    }
}
