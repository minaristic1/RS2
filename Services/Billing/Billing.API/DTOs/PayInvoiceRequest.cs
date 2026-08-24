using System.ComponentModel.DataAnnotations;
using Billing.Domain.ValueObjects;

namespace Billing.API.DTOs;

public sealed record PayInvoiceRequest(
    PaymentMethod Method,
    [Required, StringLength(100)] string Provider,
    [Required, StringLength(200)] string TransactionReference);

