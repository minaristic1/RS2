using Billing.API.Middleware;
using Billing.API.Messaging;
using Billing.API.Services;
using Billing.Application;
using Billing.Application.Contracts.Infrastructure;
using Billing.Infrastructure;
using Billing.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT token returned by UserService."
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddGrpc();
builder.Services.AddHostedService<CartCheckedOutConsumer>();
builder.Services.AddScoped<IOrderReadyForDeliveryPublisher, OrderReadyForDeliveryPublisher>();

var restaurantServiceUrl = builder.Configuration["Services:RestaurantService"]
    ?? throw new InvalidOperationException("Restaurant service URL is not configured.");
builder.Services.AddHttpClient<IRestaurantService, RestaurantServiceClient>(client =>
{
    client.BaseAddress = new Uri(restaurantServiceUrl);
});

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "GrizGo Billing API v1");
    options.RoutePrefix = "swagger";
});

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BillingContext>();
    await context.Database.EnsureCreatedAsync();
    await context.Database.ExecuteSqlRawAsync(
        """
        ALTER TABLE "Invoices"
        ADD COLUMN IF NOT EXISTS "RestaurantId" uuid NOT NULL
        DEFAULT '00000000-0000-0000-0000-000000000000';

        ALTER TABLE "Invoices"
        ADD COLUMN IF NOT EXISTS "DeliveryAddress" character varying(500)
        NOT NULL DEFAULT '';

        CREATE INDEX IF NOT EXISTS "IX_Invoices_RestaurantId"
        ON "Invoices" ("RestaurantId");
        """);
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGrpcService<BillingGrpcService>();

app.Run();
