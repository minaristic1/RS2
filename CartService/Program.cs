using CartService.Repositories;
using CartService.Services;
using StackExchange.Redis;
using Microsoft.AspNetCore.Diagnostics;
using CartService.Exceptions;
using CartService.Clients;
using CartService.Messaging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var redisConnectionString = builder.Configuration.GetConnectionString("Redis")
                            ?? throw new InvalidOperationException("Redis connection string is not configured.");

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddScoped<ICartRepository, RedisCartRepositories>();

builder.Services.AddScoped<ICartService, CartManager>();

builder.Services.AddScoped<IRabbitMqPublisher, RabbitMqPublisher>();

var restaurantServiceUrl = builder.Configuration["Services:RestaurantService"] 
                           ?? throw new InvalidOperationException("Restaurant service url is not configured.");

builder.Services.AddHttpClient<IRestaurantClient, RestaurantClient>(client =>
{
    client.BaseAddress = new Uri(restaurantServiceUrl);
});

var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exception is NotFoundException)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;

            await context.Response.WriteAsJsonAsync(new
            {
                error = exception.Message
            });
            return;
        }

        if (exception is ConflictException)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;

            await context.Response.WriteAsJsonAsync(new
            {
                error = exception.Message
            });
            return;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await context.Response.WriteAsJsonAsync(new
        {
            error = "Neočekivana greška."
        });
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();