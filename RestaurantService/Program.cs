using Microsoft.EntityFrameworkCore;
using RestaurantService.Application.Interfaces;
using RestaurantService.Infrastructure.Persistence;
using RestaurantService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("RestaurantDb")
    ?? throw new InvalidOperationException(
        "Connection string 'RestaurantDb' is not configured.");

builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
