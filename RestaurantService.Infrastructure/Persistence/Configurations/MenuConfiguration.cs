using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantService.Domain.Entities;

namespace RestaurantService.Infrastructure.Persistence.Configurations
{
    public class MenuConfiguration : IEntityTypeConfiguration<Menu>
    {
        public void Configure(EntityTypeBuilder<Menu> builder)
        {
            builder.ToTable("Menus");

            builder.HasKey(menu => menu.Id);

            builder.Property(menu => menu.NameSr)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(menu => menu.NameEn)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(menu => menu.DescriptionSr)
                .HasMaxLength(1000);

            builder.Property(menu => menu.DescriptionEn)
                .HasMaxLength(1000);

            builder.HasMany(menu => menu.Categories)
                .WithOne(category => category.Menu)
                .HasForeignKey(category => category.MenuId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
