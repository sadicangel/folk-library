using Ardalis.Specification;
using FolkLibrary.Models;

namespace FolkLibrary.Interfaces;
public interface IMongoRepository<T> : IRepositoryBase<T> where T : class, IDataTransterObject
{
    string CollectionName { get; }

    Task<Page<T>> ListPagedAsync(ISpecification<T> specification, string? continuationToken, CancellationToken cancellationToken = default);

    Task<Page<TResult>> ListPagedAsync<TResult>(ISpecification<T, TResult> specification, string? continuationToken, CancellationToken cancellationToken = default);
}