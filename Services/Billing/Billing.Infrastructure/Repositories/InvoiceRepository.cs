using Billing.Application.Contracts.Persistence;
using Billing.Domain.Aggregates;
using Billing.Domain.Entities;
using Billing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Billing.Infrastructure.Repositories;

public sealed class InvoiceRepository(BillingContext context)
    : RepositoryBase<Invoice>(context), IInvoiceRepository
{
    public Task<Invoice?> GetDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return Context.Invoices
            .Include(invoice => invoice.Items)
            .Include(invoice => invoice.Payments)
            .AsSplitQuery()
            .FirstOrDefaultAsync(invoice => invoice.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Invoice>> GetByCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Invoices
            .Include(invoice => invoice.Items)
            .Include(invoice => invoice.Payments)
            .AsSplitQuery()
            .Where(invoice => invoice.CustomerId == customerId)
            .OrderByDescending(invoice => invoice.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsForOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return Context.Invoices.AnyAsync(
            invoice => invoice.OrderId == orderId,
            cancellationToken);
    }

    public Task<bool> TransactionReferenceExistsAsync(
        string transactionReference,
        CancellationToken cancellationToken = default)
    {
        return Context.Payments.AnyAsync(
            payment => payment.TransactionReference == transactionReference,
            cancellationToken);
    }

    public async Task AddPaymentAsync(
        Invoice invoice,
        Payment payment,
        CancellationToken cancellationToken = default)
    {
        Context.Entry(payment).State = EntityState.Added;
        await Context.SaveChangesAsync(cancellationToken);
    }
}
