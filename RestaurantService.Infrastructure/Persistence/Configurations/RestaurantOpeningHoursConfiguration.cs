using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantService.Domain.Entities;

namespace RestaurantService.Infrastructure.Persistence.Configurations
{
    public class RestaurantOpeningHoursConfiguration : IEntityTypeConfiguration<RestaurantOpeningHours>
    {
        public void Configure(EntityTypeBuilder<RestaurantOpeningHours> builder)
        {
            builder.HasKey(oh => oh.Id);

            builder.Property(oh => oh.DayOfWeek)
                .IsRequired();

            builder.Property(oh => oh.OpenTime)
                .IsRequired();

            builder.Property(oh => oh.CloseTime)
                .IsRequired();

            builder.HasOne(oh => oh.Restaurant)
                .WithMany(r => r.OpeningHours)
                .HasForeignKey(oh => oh.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(oh => new { oh.RestaurantId, oh.DayOfWeek })
                .IsUnique();
        }
    }
}