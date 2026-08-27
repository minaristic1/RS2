using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using RestaurantService.Domain.Entities;

namespace RestaurantService.Infrastructure.Persistence;

public class RestaurantDbContext : DbContext
{
    public RestaurantDbContext(
        DbContextOptions<RestaurantDbContext> options)
        : base(options)
    {
    }

    public DbSet<Restaurant> Restaurants => Set<Restaurant>();

    public DbSet<Menu> Menus => Set<Menu>();

    public DbSet<MenuCategory> MenuCategories => Set<MenuCategory>();

    public DbSet<MenuItem> MenuItems => Set<MenuItem>();

    public DbSet<Promotion> Promotions => Set<Promotion>();

    public DbSet<RestaurantOpeningHours> RestaurantOpeningHours => Set<RestaurantOpeningHours>();

    public DbSet<RestaurantHolidayException> RestaurantHolidayExceptions => Set<RestaurantHolidayException>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(RestaurantDbContext).Assembly);
    }
}