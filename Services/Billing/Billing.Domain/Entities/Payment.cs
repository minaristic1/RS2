using Billing.Domain.Common;
using Billing.Domain.Exceptions;
using Billing.Domain.ValueObjects;

namespace Billing.Domain.Entities;

public sealed class Payment : EntityBase
{
    private Payment()
    {
    }

    private Payment(
        decimal amount,
        string currency,
        PaymentMethod method,
        string provider,
        string transactionReference)
    {
        if (amount <= 0)
        {
            throw new BillingDomainException("Payment amount must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(provider))
        {
            throw new BillingDomainException("Payment provider is required.");
        }

        if (string.IsNullOrWhiteSpace(transactionReference))
        {
            throw new BillingDomainException("Transaction reference is required.");
        }

        Amount = decimal.Round(amount, 2);
        Currency = currency;
        Method = method;
        Provider = provider.Trim();
        TransactionReference = transactionReference.Trim();
        Status = PaymentStatus.Completed;
        ProcessedAt = DateTimeOffset.UtcNow;
    }

    public Guid InvoiceId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string Provider { get; private set; } = string.Empty;
    public string TransactionReference { get; private set; } = string.Empty;
    public DateTimeOffset ProcessedAt { get; private set; }

    internal static Payment Complete(
        decimal amount,
        string currency,
        PaymentMethod method,
        string provider,
        string transactionReference)
    {
        return new Payment(amount, currency, method, provider, transactionReference);
    }
}

