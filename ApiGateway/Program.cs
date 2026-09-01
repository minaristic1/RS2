using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile(
        "ocelot.json",
        optional: false,
        reloadOnChange: true);

if (builder.Configuration.GetValue<bool>("UseDockerServiceDiscovery"))
{
    var downstreamServices = new Dictionary<string, (string Host, int Port)>
    {
        ["/api/carts"] = ("cart-service", 8080),
        ["/api/delivery"] = ("delivery-api", 8080),
        ["/api/restaurants"] = ("restaurant-api", 8080),
        ["/api/menu-items"] = ("restaurant-api", 8080),
        ["/api/users"] = ("user-api", 8080),
        ["/api/invoices"] = ("billing-api", 5005)
    };
    var overrides = new Dictionary<string, string?>();
    var routes = builder.Configuration.GetSection("Routes").GetChildren().ToArray();

    for (var index = 0; index < routes.Length; index++)
    {
        var downstreamPath = routes[index]["DownstreamPathTemplate"];
        var service = downstreamServices.FirstOrDefault(entry =>
            downstreamPath?.StartsWith(entry.Key, StringComparison.OrdinalIgnoreCase) == true);

        if (service.Key is null)
        {
            continue;
        }

        overrides[$"Routes:{index}:DownstreamHostAndPorts:0:Host"] = service.Value.Host;
        overrides[$"Routes:{index}:DownstreamHostAndPorts:0:Port"] =
            service.Value.Port.ToString();
    }

    builder.Configuration.AddInMemoryCollection(overrides);
}

builder.Services.AddOcelot(builder.Configuration);

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT signing key is not configured.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT issuer is not configured.");
var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT audience is not configured.");

builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseHealthChecks("/health");

app.UseCors("AllowFrontend");

app.UseAuthentication();

await app.UseOcelot();

app.Run();