using Billing.Application.Contracts.Persistence;
using Billing.Infrastructure.Persistence;
using Billing.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Billing.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BillingConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'BillingConnection' is not configured.");

        services.AddDbContext<BillingContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();

        return services;
    }
}

