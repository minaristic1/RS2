using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantService.Domain.Entities;

namespace RestaurantService.Infrastructure.Persistence.Configurations
{
    public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(EntityTypeBuilder<MenuItem> builder)
        {
            builder.ToTable("MenuItems");

            builder.HasKey(item => item.Id);

            builder.Property(item => item.NameSr)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(item => item.NameEn)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(item => item.DescriptionSr)
                .HasMaxLength(1000);

            builder.Property(item => item.DescriptionEn)
                .HasMaxLength(1000);

            builder.Property(item => item.ImageUrl)
                .HasMaxLength(1000);

            builder.Property(item => item.Price)
                .HasPrecision(18, 2);
        }
    }
}
