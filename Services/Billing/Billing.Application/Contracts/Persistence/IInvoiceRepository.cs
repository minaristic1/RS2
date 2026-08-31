using Billing.Domain.Aggregates;
using Billing.Domain.Entities;

namespace Billing.Application.Contracts.Persistence;

public interface IInvoiceRepository : IAsyncRepository<Invoice>
{
    Task<Invoice?> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Invoice>> GetByCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsForOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<bool> TransactionReferenceExistsAsync(
        string transactionReference,
        CancellationToken cancellationToken = default);
    Task AddPaymentAsync(
        Invoice invoice,
        Payment payment,
        CancellationToken cancellationToken = default);
}
