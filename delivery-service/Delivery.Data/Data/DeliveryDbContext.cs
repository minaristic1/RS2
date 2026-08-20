using Microsoft.EntityFrameworkCore;
using Delivery.Data.Models;

namespace Delivery.Data
{
    public class DeliveryDbContext : DbContext
    {
        public DeliveryDbContext(DbContextOptions<DeliveryDbContext> options) : base(options)
        {
        }

        public DbSet<DeliveryOrder> Deliveries { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}
