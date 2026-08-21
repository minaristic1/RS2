using Ocelot.DependencyInjection;
using Ocelot.Middleware;
 
var builder = WebApplication.CreateBuilder(args);
 
builder.Configuration
    .AddJsonFile(
        "ocelot.json",
        optional: false,
        reloadOnChange: true);
 
builder.Services.AddOcelot(builder.Configuration);
 
builder.Services.AddHealthChecks();
 
var app = builder.Build();
 
app.UseHealthChecks("/health");
 
await app.UseOcelot();
 
app.Run();