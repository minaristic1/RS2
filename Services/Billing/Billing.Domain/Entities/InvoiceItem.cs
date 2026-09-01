using Billing.Domain.Common;
using Billing.Domain.Exceptions;

namespace Billing.Domain.Entities;

public sealed class InvoiceItem : EntityBase
{
    private InvoiceItem()
    {
    }

    private InvoiceItem(Guid productId, string name, int quantity, decimal unitPrice)
    {
        if (productId == Guid.Empty)
        {
            throw new BillingDomainException("Product identifier is required.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BillingDomainException("Product name is required.");
        }

        if (quantity <= 0)
        {
            throw new BillingDomainException("Quantity must be greater than zero.");
        }

        if (unitPrice < 0)
        {
            throw new BillingDomainException("Unit price cannot be negative.");
        }

        ProductId = productId;
        Name = name.Trim();
        Quantity = quantity;
        UnitPrice = decimal.Round(unitPrice, 2);
    }

    public Guid InvoiceId { get; private set; }
    public Guid ProductId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Total => Quantity * UnitPrice;

    public static InvoiceItem Create(Guid productId, string name, int quantity, decimal unitPrice)
    {
        return new InvoiceItem(productId, name, quantity, unitPrice);
    }
}

