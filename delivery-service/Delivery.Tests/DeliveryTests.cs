using System;
using Delivery.Api.Models;
using Xunit;

namespace Delivery.Tests
{
    public class DeliveryTests
    {
        [Fact]
        public void NewDelivery_StartsWithCreatedStatus()
        {
            var delivery = new DeliveryOrder(Guid.NewGuid(), "Bulevar Kralja Aleksandra 1");

            Assert.Equal(DeliveryStatus.Created, delivery.Status);
        }

        [Fact]
        public void AdvanceStatus_MovesToNextStatusInSequence()
        {
            var delivery = new DeliveryOrder(Guid.NewGuid(), "Bulevar Kralja Aleksandra 1");

            delivery.AdvanceStatus();

            Assert.Equal(DeliveryStatus.Confirmed, delivery.Status);
        }

        [Fact]
        public void AdvanceStatus_AfterDelivered_ThrowsInvalidOperationException()
        {
            var delivery = new DeliveryOrder(Guid.NewGuid(), "Bulevar Kralja Aleksandra 1");
            delivery.AdvanceStatus(); // Confirmed
            delivery.AdvanceStatus(); // Preparing
            delivery.AdvanceStatus(); // OutForDelivery
            delivery.AdvanceStatus(); // Delivered

            Assert.Throws<InvalidOperationException>(() => delivery.AdvanceStatus());
        }

        [Fact]
        public void AdvanceStatus_AfterCancelled_ThrowsInvalidOperationException()
        {
            var delivery = new DeliveryOrder(Guid.NewGuid(), "Bulevar Kralja Aleksandra 1");
            delivery.Cancel();

            Assert.Throws<InvalidOperationException>(() => delivery.AdvanceStatus());
        }

        [Fact]
        public void AdvanceStatus_ToDelivered_SetsDeliveredAt()
        {
            var delivery = new DeliveryOrder(Guid.NewGuid(), "Bulevar Kralja Aleksandra 1");

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
            var delivery = new DeliveryOrder(Guid.NewGuid(), "Bulevar Kralja Aleksandra 1");

            delivery.Cancel();

            Assert.Equal(DeliveryStatus.Cancelled, delivery.Status);
            Assert.NotNull(delivery.CancelledAt);
        }

        [Fact]
        public void Cancel_AfterDelivered_ThrowsInvalidOperationException()
        {
            var delivery = new DeliveryOrder(Guid.NewGuid(), "Bulevar Kralja Aleksandra 1");
            delivery.AdvanceStatus();
            delivery.AdvanceStatus();
            delivery.AdvanceStatus();
            delivery.AdvanceStatus(); // Delivered

            Assert.Throws<InvalidOperationException>(() => delivery.Cancel());
        }
    }
}
