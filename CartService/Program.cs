using CartService.Repositories;
using CartService.Services;
using StackExchange.Redis;
using Microsoft.AspNetCore.Diagnostics;
using CartService.Exceptions;
using CartService.Clients;
using CartService.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var redisConnectionString = builder.Configuration.GetConnectionString("Redis")
                            ?? throw new InvalidOperationException("Redis connection string is not configured.");

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddScoped<ICartRepository, RedisCartRepositories>();

builder.Services.AddScoped<ICartService, CartManager>();

builder.Services.AddScoped<IRabbitMqPublisher, RabbitMqPublisher>();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT signing key is not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();