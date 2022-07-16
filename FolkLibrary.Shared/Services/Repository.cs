using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FolkLibrary.Models;

namespace FolkLibrary.Services;

public sealed class Repository<T> : RepositoryBase<T>, IRepository<T>
    where T : Item
{
    public Repository(FolkLibraryContext context) : base(context)
    {
    }

    async ValueTask<T?> IRepository<T>.GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        return entity;
    }
    async ValueTask<T?> IRepository<T>.GetAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken)
    {
        var entity = await SingleOrDefaultAsync(specification, cancellationToken);
        return entity;
    }

    async ValueTask<IList<T>> IRepository<T>.GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await ListAsync(cancellationToken);
        return entities;
    }

    async ValueTask<IList<T>> IRepository<T>.GetAllAsync(ISpecification<T> specification, CancellationToken cancellationToken)
    {
        var entities = await ListAsync(specification, cancellationToken);
        return entities;
    }

    async ValueTask<T> IRepository<T>.InsertAsync(T entity, CancellationToken cancellationToken)
    {
        return await AddAsync(entity, cancellationToken);
    }

    async ValueTask IRepository<T>.UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        await UpdateAsync(entity, cancellationToken);
    }
    async ValueTask IRepository<T>.UpsertAsync(T entity, CancellationToken cancellationToken)
    {
        var result = await GetByIdAsync(entity.Id, cancellationToken);
        if (result is not null)
            await UpdateAsync(entity, cancellationToken);
        else
            await AddAsync(entity, cancellationToken);
    }

    async ValueTask IRepository<T>.DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await GetByIdAsync(id, cancellationToken);
        if (result is not null)
            await DeleteAsync(result, cancellationToken);
    }
}
