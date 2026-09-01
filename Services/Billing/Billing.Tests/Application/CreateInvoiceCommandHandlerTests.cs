using Billing.Application.Contracts.Persistence;
using Billing.Application.Features.Billing.Commands.CreateInvoice;
using Billing.Domain.Aggregates;
using Moq;

namespace Billing.Tests.Application;

public sealed class CreateInvoiceCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreatesInvoiceWithDeliveryDetails()
    {
        var repository = new Mock<IInvoiceRepository>();
        repository
            .Setup(value => value.ExistsForOrderAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repository
            .Setup(value => value.AddAsync(
                It.IsAny<Invoice>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice invoice, CancellationToken _) => invoice);

        var command = new CreateInvoiceCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Kralja Petra 10",
            "RSD",
            [
                new CreateInvoiceItem(
                    Guid.NewGuid(),
                    "Burger",
                    2,
                    600)
            ]);

        var handler = new CreateInvoiceCommandHandler(repository.Object);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(command.RestaurantId, result.RestaurantId);
        Assert.Equal(command.DeliveryAddress, result.DeliveryAddress);
        Assert.Equal(1200, result.TotalAmount);
        repository.Verify(value => value.AddAsync(
            It.Is<Invoice>(invoice =>
                invoice.RestaurantId == command.RestaurantId &&
                invoice.DeliveryAddress == command.DeliveryAddress),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}

