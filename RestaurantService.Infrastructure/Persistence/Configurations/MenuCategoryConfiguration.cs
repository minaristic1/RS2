using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantService.Domain.Entities;

namespace RestaurantService.Infrastructure.Persistence.Configurations
{
    public class MenuCategoryConfiguration : IEntityTypeConfiguration<MenuCategory>
    {
        public void Configure(EntityTypeBuilder<MenuCategory> builder)
        {
            builder.ToTable("MenuCategories");

            builder.HasKey(category => category.Id);

            builder.Property(category => category.NameSr)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(category => category.NameEn)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(category => category.DescriptionSr)
                .HasMaxLength(1000);

            builder.Property(category => category.DescriptionEn)
                .HasMaxLength(1000);

            builder.HasMany(category => category.Items)
                .WithOne(item => item.MenuCategory)
                .HasForeignKey(item => item.MenuCategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}