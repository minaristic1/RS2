using Delivery.Data;
using Delivery.GRPC.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8081, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
});

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddDbContext<DeliveryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DeliveryDb")));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<DeliveryGrpcServiceImpl>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
