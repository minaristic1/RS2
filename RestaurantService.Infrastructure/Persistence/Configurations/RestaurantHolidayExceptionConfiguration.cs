using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantService.Domain.Entities;

namespace RestaurantService.Infrastructure.Persistence.Configurations
{
    public class RestaurantHolidayExceptionConfiguration : IEntityTypeConfiguration<RestaurantHolidayException>
    {
        public void Configure(EntityTypeBuilder<RestaurantHolidayException> builder)
        {
            builder.HasKey(exception => exception.Id);

            builder.Property(exception => exception.Date)
                .IsRequired();

            builder.HasOne(exception => exception.Restaurant)
                .WithMany(r => r.HolidayExceptions)
                .HasForeignKey(exception => exception.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(exception => new { exception.RestaurantId, exception.Date })
                .IsUnique();
        }
    }
}