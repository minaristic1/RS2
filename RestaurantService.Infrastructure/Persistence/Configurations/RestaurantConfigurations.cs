using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantService.Domain.Entities;

namespace RestaurantService.Infrastructure.Persistence.Configurations
{
    public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
    {
        public void Configure(EntityTypeBuilder<Restaurant> builder)
        {
            builder.ToTable("Restaurants");

            builder.HasKey(restaurant => restaurant.Id);

            builder.Property(restaurant => restaurant.NameSr)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(restaurant => restaurant.NameEn)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(restaurant => restaurant.Address)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(restaurant => restaurant.CuisineType)
                .HasMaxLength(100);

            builder.HasMany(restaurant => restaurant.Menus)
                .WithOne(menu => menu.Restaurant)
                .HasForeignKey(menu => menu.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
