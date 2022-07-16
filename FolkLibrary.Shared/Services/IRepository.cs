using Ardalis.Specification;
using FolkLibrary.Models;

namespace FolkLibrary.Services;

public interface IRepository<T>
    where T : Item
{
    ValueTask<T?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    ValueTask<T?> GetAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default);
    ValueTask<IList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    ValueTask<IList<T>> GetAllAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    ValueTask<T> InsertAsync(T entity, CancellationToken cancellationToken = default);
    ValueTask UpdateAsync(T entity, CancellationToken cancellationToken = default);
    ValueTask UpsertAsync(T entity, CancellationToken cancellationToken = default);
    ValueTask DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
