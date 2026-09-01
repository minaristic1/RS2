using Billing.Application.Contracts.Infrastructure;
using Billing.Application.Contracts.Persistence;
using Billing.Application.Exceptions;
using Billing.Application.Features.Billing.Commands.PayInvoice;
using Billing.Application.Models;
using Billing.Domain.Aggregates;
using Billing.Domain.Entities;
using Billing.Domain.ValueObjects;
using Moq;

namespace Billing.Tests.Application;

public sealed class PayInvoiceCommandHandlerTests
{
    [Fact]
    public async Task Handle_PersistsPaymentAndPublishesDelivery()
    {
        var invoice = CreateInvoice();
        var restaurant = new RestaurantInfo(
            invoice.RestaurantId,
            "Test restoran",
            "Bulevar kralja Aleksandra 1");
        var repository = CreateRepository(invoice);
        var restaurantService = new Mock<IRestaurantService>();
        var publisher = new Mock<IOrderReadyForDeliveryPublisher>();

        restaurantService
            .Setup(value => value.GetRestaurantAsync(
                invoice.RestaurantId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(restaurant);

        var handler = new PayInvoiceCommandHandler(
            repository.Object,
            restaurantService.Object,
            publisher.Object);
        var command = new PayInvoiceCommand(
            invoice.Id,
            PaymentMethod.Card,
            "GrizGo",
            "transaction-1",
            "Petar Petrovic",
            "0601234567");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(PaymentStatus.Completed.ToString(), result.Status);
        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
        repository.Verify(value => value.AddPaymentAsync(
            invoice,
            It.Is<Payment>(payment =>
                payment.Status == PaymentStatus.Completed &&
                payment.Amount == invoice.TotalAmount),
            It.IsAny<CancellationToken>()), Times.Once);
        publisher.Verify(value => value.PublishAsync(
            invoice,
            restaurant,
            command.CustomerName,
            command.CustomerPhone,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRestaurantDoesNotExist_DoesNotPublishDelivery()
    {
        var invoice = CreateInvoice();
        var repository = CreateRepository(invoice);
        var restaurantService = new Mock<IRestaurantService>();
        var publisher = new Mock<IOrderReadyForDeliveryPublisher>();

        restaurantService
            .Setup(value => value.GetRestaurantAsync(
                invoice.RestaurantId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((RestaurantInfo?)null);

        var handler = new PayInvoiceCommandHandler(
            repository.Object,
            restaurantService.Object,
            publisher.Object);
        var command = new PayInvoiceCommand(
            invoice.Id,
            PaymentMethod.Card,
            "GrizGo",
            "transaction-2",
            "Petar Petrovic",
            "0601234567");

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));

        Assert.Equal(InvoiceStatus.AwaitingPayment, invoice.Status);
        repository.Verify(value => value.AddPaymentAsync(
            It.IsAny<Invoice>(),
            It.IsAny<Payment>(),
            It.IsAny<CancellationToken>()), Times.Never);
        publisher.Verify(value => value.PublishAsync(
            It.IsAny<Invoice>(),
            It.IsAny<RestaurantInfo>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IInvoiceRepository> CreateRepository(Invoice invoice)
    {
        var repository = new Mock<IInvoiceRepository>();
        repository
            .Setup(value => value.GetDetailsAsync(
                invoice.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        repository
            .Setup(value => value.TransactionReferenceExistsAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        return repository;
    }

    private static Invoice CreateInvoice()
    {
        return Invoice.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Studentski trg 16",
            "RSD",
            [
                InvoiceItem.Create(Guid.NewGuid(), "Pizza", 2, 750)
            ]);
    }
}

