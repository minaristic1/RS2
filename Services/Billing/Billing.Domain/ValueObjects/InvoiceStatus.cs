namespace Billing.Domain.ValueObjects;

public enum InvoiceStatus
{
    AwaitingPayment = 1,
    Paid = 2,
    Cancelled = 3
}

