using Billing.API.DTOs;
using Billing.Application.Features.Billing.Commands.CreateInvoice;
using Billing.Application.Features.Billing.Commands.PayInvoice;
using Billing.Application.Features.Billing.Queries.GetCustomerInvoices;
using Billing.Application.Features.Billing.Queries.GetInvoice;
using Billing.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Billing.API.Controllers;

[ApiController]
[Authorize]
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
        var customerId = GetCurrentUserId();
        var command = new CreateInvoiceCommand(
            request.OrderId,
            customerId,
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
        var invoice = await sender.Send(new GetInvoiceQuery(id), cancellationToken);
        return CanAccess(invoice.CustomerId) ? Ok(invoice) : Forbid();
    }

    [HttpGet("customer/{customerId:guid}")]
    [ProducesResponseType<IReadOnlyCollection<InvoiceDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<InvoiceDto>>> GetCustomerInvoices(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        if (!CanAccess(customerId))
        {
            return Forbid();
        }

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
        var invoice = await sender.Send(new GetInvoiceQuery(id), cancellationToken);
        if (!CanAccess(invoice.CustomerId))
        {
            return Forbid();
        }

        var payment = await sender.Send(
            new PayInvoiceCommand(
                id,
                request.Method,
                request.Provider,
                request.TransactionReference),
            cancellationToken);

        return Ok(payment);
    }

    private Guid GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId)
            ? userId
            : throw new UnauthorizedAccessException("Authenticated user identifier is invalid.");
    }

    private bool CanAccess(Guid customerId)
    {
        return User.IsInRole("Admin") || GetCurrentUserId() == customerId;
    }
}
