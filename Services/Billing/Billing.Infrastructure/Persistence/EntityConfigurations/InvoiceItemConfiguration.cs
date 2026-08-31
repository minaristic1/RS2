using Billing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Infrastructure.Persistence.EntityConfigurations;

public sealed class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("InvoiceItems");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Name).HasMaxLength(200).IsRequired();
        builder.Property(item => item.UnitPrice).HasPrecision(18, 2);
        builder.Ignore(item => item.Total);
    }
}

