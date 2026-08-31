using Billing.Application.Contracts.Persistence;
using Billing.Domain.Aggregates;
using Billing.Domain.Entities;
using Billing.Domain.Exceptions;
using MediatR;

namespace Billing.Application.Features.Billing.Commands.CreateInvoice;

public sealed class CreateInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
    : IRequestHandler<CreateInvoiceCommand, Models.InvoiceDto>
{
    public async Task<Models.InvoiceDto> Handle(
        CreateInvoiceCommand request,
        CancellationToken cancellationToken)
    {
        if (await invoiceRepository.ExistsForOrderAsync(request.OrderId, cancellationToken))
        {
            throw new BillingDomainException("An invoice already exists for this order.");
        }

        var items = request.Items.Select(item =>
            InvoiceItem.Create(item.ProductId, item.Name, item.Quantity, item.UnitPrice));

        var invoice = Invoice.Create(
            request.OrderId,
            request.CustomerId,
            request.Currency,
            items);

        await invoiceRepository.AddAsync(invoice, cancellationToken);

        return Models.InvoiceDto.FromInvoice(invoice);
    }
}

