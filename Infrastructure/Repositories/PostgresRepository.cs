using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FolkLibrary.Database;
using Microsoft.EntityFrameworkCore;

namespace FolkLibrary.Repositories;

internal abstract class PostgresRepository<TEntity> : RepositoryBase<TEntity>, IRepository<TEntity> where TEntity : class
{
    private readonly FolkDbContext _context;

    public string CollectionName { get; }

    public PostgresRepository(FolkDbContext context) : base(context)
    {
        CollectionName = context.Model.FindEntityType(typeof(TEntity))!.GetTableName()!;
        _context = context;
    }

    public async Task<Page<TEntity>> ListAsync(ISpecification<TEntity> specification, int pageIndex, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var set = _context.Set<TEntity>().WithSpecification(specification);
        var pageCount = await set.CountAsync(cancellationToken) / pageSize;
        var hasMoreResults = pageIndex + 1 < pageCount;

        return new Page<TEntity>
        {
            PageIndex = pageIndex,
            HasMoreResults = hasMoreResults,
            Items = await set.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken)
        };
    }

    public async Task<Page<TResult>> ListAsync<TResult>(ISpecification<TEntity, TResult> specification, int pageIndex, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var set = _context.Set<TEntity>().WithSpecification(specification);
        var pageCount = await set.CountAsync(cancellationToken) / pageSize;
        var hasMoreResults = pageIndex + 1 < pageCount;

        return new Page<TResult>
        {
            PageIndex = pageIndex,
            HasMoreResults = hasMoreResults,
            Items = await set.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken)
        };
    }
}
