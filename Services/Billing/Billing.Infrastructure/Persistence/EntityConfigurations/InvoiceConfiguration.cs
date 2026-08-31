using Billing.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Infrastructure.Persistence.EntityConfigurations;

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");
        builder.HasKey(invoice => invoice.Id);
        builder.HasIndex(invoice => invoice.OrderId).IsUnique();
        builder.HasIndex(invoice => invoice.CustomerId);

        builder.Property(invoice => invoice.Currency).HasMaxLength(3).IsRequired();
        builder.Property(invoice => invoice.TotalAmount).HasPrecision(18, 2);
        builder.Property(invoice => invoice.Status).HasConversion<string>().HasMaxLength(30);

        builder.HasMany(invoice => invoice.Items)
            .WithOne()
            .HasForeignKey(item => item.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(invoice => invoice.Payments)
            .WithOne()
            .HasForeignKey(payment => payment.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(invoice => invoice.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(invoice => invoice.Payments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

