using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantService.Domain.Entities;

namespace RestaurantService.Infrastructure.Persistence.Configurations
{
    public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.ToTable("Promotions");

            builder.HasKey(promotion => promotion.Id);

            builder.Property(promotion => promotion.NameSr)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(promotion => promotion.NameEn)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(promotion => promotion.DiscountPercentage)
                .HasPrecision(5, 2);

            builder.HasOne(promotion => promotion.MenuItem)
                .WithMany(item => item.Promotions)
                .HasForeignKey(promotion => promotion.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(promotion => promotion.MenuCategory)
                .WithMany(category => category.Promotions)
                .HasForeignKey(promotion => promotion.MenuCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(promotion => promotion.Menu)
                .WithMany(menu => menu.Promotions)
                .HasForeignKey(promotion => promotion.MenuId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable(table => table.HasCheckConstraint(
                "CK_Promotion_ExactlyOneTarget",
                "(CASE WHEN MenuItemId IS NOT NULL THEN 1 ELSE 0 END) + " +
                "(CASE WHEN MenuCategoryId IS NOT NULL THEN 1 ELSE 0 END) + " +
                "(CASE WHEN MenuId IS NOT NULL THEN 1 ELSE 0 END) = 1"));
            }
    }
}