using Billing.API.DTOs;
using Billing.Application.Features.Billing.Commands.CreateInvoice;
using Billing.Application.Features.Billing.Commands.PayInvoice;
using Billing.Application.Features.Billing.Queries.GetCustomerInvoices;
using Billing.Application.Features.Billing.Queries.GetInvoice;
using Billing.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Billing.API.Controllers;

[ApiController]
[Route("api/invoices")]
public sealed class InvoicesController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<InvoiceDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InvoiceDto>> CreateInvoice(
        CreateInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateInvoiceCommand(
            request.OrderId,
            request.CustomerId,
            request.Currency,
            request.Items.Select(item => new CreateInvoiceItem(
                item.ProductId,
                item.Name,
                item.Quantity,
                item.UnitPrice)).ToArray());

        var invoice = await sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<InvoiceDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(
        Guid id,
        CancellationToken cancellationToken)
    {
        return Ok(await sender.Send(new GetInvoiceQuery(id), cancellationToken));
    }

    [HttpGet("customer/{customerId:guid}")]
    [ProducesResponseType<IReadOnlyCollection<InvoiceDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<InvoiceDto>>> GetCustomerInvoices(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        return Ok(await sender.Send(
            new GetCustomerInvoicesQuery(customerId),
            cancellationToken));
    }

    [HttpPost("{id:guid}/payments")]
    [ProducesResponseType<PaymentDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentDto>> PayInvoice(
        Guid id,
        PayInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var payment = await sender.Send(
            new PayInvoiceCommand(
                id,
                request.Method,
                request.Provider,
                request.TransactionReference),
            cancellationToken);

        return Ok(payment);
    }
}

