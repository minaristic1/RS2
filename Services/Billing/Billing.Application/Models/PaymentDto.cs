using Billing.Domain.Entities;

namespace Billing.Application.Models;

public sealed record PaymentDto(
    Guid Id,
    decimal Amount,
    string Currency,
    string Method,
    string Status,
    string Provider,
    string TransactionReference,
    DateTimeOffset ProcessedAt)
{
    public static PaymentDto FromPayment(Payment payment)
    {
        return new PaymentDto(
            payment.Id,
            payment.Amount,
            payment.Currency,
            payment.Method.ToString(),
            payment.Status.ToString(),
            payment.Provider,
            payment.TransactionReference,
            payment.ProcessedAt);
    }
}

