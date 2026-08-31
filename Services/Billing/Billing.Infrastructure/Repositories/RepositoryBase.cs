using System.Linq.Expressions;
using Billing.Application.Contracts.Persistence;
using Billing.Domain.Common;
using Billing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Billing.Infrastructure.Repositories;

public class RepositoryBase<T>(BillingContext context) : IAsyncRepository<T>
    where T : AggregateRoot
{
    protected BillingContext Context { get; } = context;

    public virtual Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return Context.Set<T>().FirstOrDefaultAsync(
            entity => entity.Id == id,
            cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>()
            .Where(predicate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await Context.Set<T>().AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (Context.Entry(entity).State == EntityState.Detached)
        {
            Context.Set<T>().Update(entity);
        }

        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        Context.Set<T>().Remove(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }
}
