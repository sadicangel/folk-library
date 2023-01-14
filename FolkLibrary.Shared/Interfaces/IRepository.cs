using Ardalis.Specification;

namespace FolkLibrary.Interfaces;
public interface IRepository<T> : IRepositoryBase<T> where T : class, IIdObject
{
    string CollectionName { get; }

    Task<Page<T>> ListPagedAsync(ISpecification<T> specification, string? continuationToken, CancellationToken cancellationToken = default);

    Task<Page<TResult>> ListPagedAsync<TResult>(ISpecification<T, TResult> specification, string? continuationToken, CancellationToken cancellationToken = default);
}