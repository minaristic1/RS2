using Billing.Domain.Common;
using Billing.Domain.Entities;
using Billing.Domain.Exceptions;
using Billing.Domain.ValueObjects;

namespace Billing.Domain.Aggregates;

public sealed class Invoice : AggregateRoot
{
    private readonly List<InvoiceItem> _items = [];
    private readonly List<Payment> _payments = [];

    private Invoice()
    {
    }

    private Invoice(Guid orderId, Guid customerId, string currency)
    {
        if (orderId == Guid.Empty)
        {
            throw new BillingDomainException("Order identifier is required.");
        }

        if (customerId == Guid.Empty)
        {
            throw new BillingDomainException("Customer identifier is required.");
        }

        if (string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3)
        {
            throw new BillingDomainException("Currency must be a three-letter ISO code.");
        }

        OrderId = orderId;
        CustomerId = customerId;
        Currency = currency.Trim().ToUpperInvariant();
        Status = InvoiceStatus.AwaitingPayment;
    }

    public Guid OrderId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public DateTimeOffset? PaidAt { get; private set; }
    public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    public static Invoice Create(
        Guid orderId,
        Guid customerId,
        string currency,
        IEnumerable<InvoiceItem> items)
    {
        var invoice = new Invoice(orderId, customerId, currency);
        invoice._items.AddRange(items);

        if (invoice._items.Count == 0)
        {
            throw new BillingDomainException("Invoice must contain at least one item.");
        }

        invoice.TotalAmount = decimal.Round(invoice._items.Sum(item => item.Total), 2);

        if (invoice.TotalAmount <= 0)
        {
            throw new BillingDomainException("Invoice total must be greater than zero.");
        }

        return invoice;
    }

    public Payment RecordPayment(
        PaymentMethod method,
        string provider,
        string transactionReference)
    {
        if (Status != InvoiceStatus.AwaitingPayment)
        {
            throw new BillingDomainException("Only an invoice awaiting payment can be paid.");
        }

        var payment = Payment.Complete(
            TotalAmount,
            Currency,
            method,
            provider,
            transactionReference);

        _payments.Add(payment);
        Status = InvoiceStatus.Paid;
        PaidAt = payment.ProcessedAt;
        MarkAsUpdated();

        return payment;
    }
}

