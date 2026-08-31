using Billing.Application.Contracts.Persistence;
using Billing.Application.Exceptions;
using Billing.Application.Models;
using MediatR;

namespace Billing.Application.Features.Billing.Queries.GetInvoice;

public sealed class GetInvoiceQueryHandler(IInvoiceRepository invoiceRepository)
    : IRequestHandler<GetInvoiceQuery, InvoiceDto>
{
    public async Task<InvoiceDto> Handle(
        GetInvoiceQuery request,
        CancellationToken cancellationToken)
    {
        var invoice = await invoiceRepository.GetDetailsAsync(
                request.InvoiceId,
                cancellationToken)
            ?? throw new NotFoundException($"Invoice '{request.InvoiceId}' was not found.");

        return InvoiceDto.FromInvoice(invoice);
    }
}

