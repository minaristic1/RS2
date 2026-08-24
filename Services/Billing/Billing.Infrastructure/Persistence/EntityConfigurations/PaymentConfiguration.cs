using Billing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Infrastructure.Persistence.EntityConfigurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(payment => payment.Id);
        builder.HasIndex(payment => payment.TransactionReference).IsUnique();

        builder.Property(payment => payment.Amount).HasPrecision(18, 2);
        builder.Property(payment => payment.Currency).HasMaxLength(3).IsRequired();
        builder.Property(payment => payment.Method).HasConversion<string>().HasMaxLength(30);
        builder.Property(payment => payment.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(payment => payment.Provider).HasMaxLength(100).IsRequired();
        builder.Property(payment => payment.TransactionReference)
            .HasMaxLength(200)
            .IsRequired();
    }
}

