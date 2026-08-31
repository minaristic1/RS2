using Ocelot.DependencyInjection;
using Ocelot.Middleware;
 
var builder = WebApplication.CreateBuilder(args);

var ocelotConfigurationFile =
    builder.Configuration["OcelotConfigurationFile"] ?? "ocelot.json";

builder.Configuration
    .AddJsonFile(
        ocelotConfigurationFile,
        optional: false,
        reloadOnChange: true);
 
builder.Services.AddOcelot(builder.Configuration);

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

await app.UseOcelot();
 
app.Run();