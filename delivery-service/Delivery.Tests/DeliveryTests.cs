using System;
using System.Collections.Generic;
using Delivery.Api.Models;
using Xunit;

namespace Delivery.Tests
{
    public class DeliveryTests
    {
        private DeliveryOrder CreateTestDelivery()
        {
            var items = new List<OrderItem>
            {
                new OrderItem { Id = Guid.NewGuid(), ProductName = "Pizza", Quantity = 1, UnitPrice = 800 }
            };

            var delivery = new DeliveryOrder(
                orderId: Guid.NewGuid(),
                customerName: "Marko Marković",
                customerPhone: "0611234567",
                restaurantId: Guid.NewGuid(),
                restaurantName: "Pizzeria Roma",
                pickupAddress: "Restoranska 1",
                deliveryAddress: "Bulevar Kralja Aleksandra 1",
                totalPrice: 800
            );
            delivery.Items = items;

            return delivery;
        }

        [Fact]
        public void NewDelivery_StartsWithCreatedStatus()
        {
            var delivery = CreateTestDelivery();

            Assert.Equal(DeliveryStatus.Created, delivery.Status);
        }

        [Fact]
        public void AdvanceStatus_MovesToNextStatusInSequence()
        {
            var delivery = CreateTestDelivery();

            delivery.AdvanceStatus();

            Assert.Equal(DeliveryStatus.Confirmed, delivery.Status);
        }

        [Fact]
        public void AdvanceStatus_AfterDelivered_ThrowsInvalidOperationException()
        {
            var delivery = CreateTestDelivery();
            delivery.AdvanceStatus(); // Confirmed
            delivery.AdvanceStatus(); // Preparing
            delivery.AdvanceStatus(); // OutForDelivery
            delivery.AdvanceStatus(); // Delivered

            Assert.Throws<InvalidOperationException>(() => delivery.AdvanceStatus());
        }

        [Fact]
        public void AdvanceStatus_AfterCancelled_ThrowsInvalidOperationException()
        {
            var delivery = CreateTestDelivery();
            delivery.Cancel();

            Assert.Throws<InvalidOperationException>(() => delivery.AdvanceStatus());
        }

        [Fact]
        public void AdvanceStatus_ToDelivered_SetsDeliveredAt()
        {
            var delivery = CreateTestDelivery();

            Assert.Null(delivery.DeliveredAt);

            delivery.AdvanceStatus(); // Confirmed
            delivery.AdvanceStatus(); // Preparing
            delivery.AdvanceStatus(); // OutForDelivery
            delivery.AdvanceStatus(); // Delivered

            Assert.NotNull(delivery.DeliveredAt);
        }

        [Fact]
        public void Cancel_SetsStatusAndCancelledAt()
        {
            var delivery = CreateTestDelivery();

            delivery.Cancel();

            Assert.Equal(DeliveryStatus.Cancelled, delivery.Status);
            Assert.NotNull(delivery.CancelledAt);
        }

        [Fact]
        public void Cancel_AfterDelivered_ThrowsInvalidOperationException()
        {
            var delivery = CreateTestDelivery();
            delivery.AdvanceStatus();
            delivery.AdvanceStatus();
            delivery.AdvanceStatus();
            delivery.AdvanceStatus(); // Delivered

            Assert.Throws<InvalidOperationException>(() => delivery.Cancel());
        }
    }
}
