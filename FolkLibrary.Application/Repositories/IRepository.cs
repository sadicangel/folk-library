using Ardalis.Specification;

namespace FolkLibrary.Repositories;

public interface IRepository<T> : IRepositoryBase<T> where T : class
{
    string CollectionName { get; }

    Task<Page<T>> ListAsync(ISpecification<T> specification, string? continuationToken, int pageSize = 20, CancellationToken cancellationToken = default);

    Task<Page<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, string? continuationToken, int pageSize = 20, CancellationToken cancellationToken = default);
}