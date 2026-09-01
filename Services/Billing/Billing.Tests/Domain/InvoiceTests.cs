using Billing.Domain.Aggregates;
using Billing.Domain.Entities;
using Billing.Domain.Exceptions;

namespace Billing.Tests.Domain;

public sealed class InvoiceTests
{
    [Fact]
    public void Create_StoresDeliveryDetailsAndCalculatesTotal()
    {
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var restaurantId = Guid.NewGuid();
        var items = new[]
        {
            InvoiceItem.Create(Guid.NewGuid(), "Pizza", 2, 750)
        };

        var invoice = Invoice.Create(
            orderId,
            customerId,
            restaurantId,
            "Studentski trg 16",
            "rsd",
            items);

        Assert.Equal(orderId, invoice.OrderId);
        Assert.Equal(customerId, invoice.CustomerId);
        Assert.Equal(restaurantId, invoice.RestaurantId);
        Assert.Equal("Studentski trg 16", invoice.DeliveryAddress);
        Assert.Equal("RSD", invoice.Currency);
        Assert.Equal(1500, invoice.TotalAmount);
    }

    [Fact]
    public void Create_WithoutDeliveryAddress_ThrowsDomainException()
    {
        var items = new[]
        {
            InvoiceItem.Create(Guid.NewGuid(), "Pizza", 1, 750)
        };

        Assert.Throws<BillingDomainException>(() => Invoice.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            string.Empty,
            "RSD",
            items));
    }
}

