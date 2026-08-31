using Billing.Application.Contracts.Persistence;
using Billing.Application.Models;
using MediatR;

namespace Billing.Application.Features.Billing.Queries.GetCustomerInvoices;

public sealed class GetCustomerInvoicesQueryHandler(IInvoiceRepository invoiceRepository)
    : IRequestHandler<GetCustomerInvoicesQuery, IReadOnlyCollection<InvoiceDto>>
{
    public async Task<IReadOnlyCollection<InvoiceDto>> Handle(
        GetCustomerInvoicesQuery request,
        CancellationToken cancellationToken)
    {
        var invoices = await invoiceRepository.GetByCustomerAsync(
            request.CustomerId,
            cancellationToken);

        return invoices.Select(InvoiceDto.FromInvoice).ToArray();
    }
}

