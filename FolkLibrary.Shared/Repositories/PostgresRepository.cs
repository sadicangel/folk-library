using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FolkLibrary.Interfaces;
using FolkLibrary.Services;
using HashidsNet;
using Microsoft.EntityFrameworkCore;

namespace FolkLibrary.Repositories;

internal abstract class PostgresRepository<TId, TEntity> : RepositoryBase<TEntity>, IRepository<TEntity>
    where TEntity : Entity<TId>
    where TId : IId<TId>
{
    private const int PageSize = 20;
    private readonly FolkDbContext _context;
    private readonly IHashids _hashService;

    public string CollectionName { get; }

    public PostgresRepository(FolkDbContext context) : base(context)
    {
        _context = context;
        CollectionName = context.Model.FindEntityType(typeof(TEntity))!.GetTableName()!;
        _hashService = new Hashids(CollectionName, minHashLength: 16);
    }

    private string EncodePageIndex(int pageIndex)
    {
        return _hashService.Encode(pageIndex);
    }

    private bool TryDecodePageIndex(string? token, out int pageIndex)
    {
        pageIndex = 0;

        if (string.IsNullOrWhiteSpace(token))
            return false;

        var ints = _hashService.Decode(token);

        if (ints.Length != 1)
            return false;

        pageIndex = ints[0];

        return true;
    }

    public async Task<Page<TEntity>> ListPagedAsync(ISpecification<TEntity> specification, string? continuationToken, CancellationToken cancellationToken = default)
    {
        TryDecodePageIndex(continuationToken, out int pageIndex);

        var set = _context.Set<TEntity>().WithSpecification(specification);
        var pageCount = await set.CountAsync(cancellationToken: cancellationToken) / PageSize;
        var nexToken = pageIndex + 1 < pageCount ? EncodePageIndex(pageIndex + 1) : null;

        return new Page<TEntity>
        {
            ContinuationToken = nexToken,
            Elements = await set.Skip(pageIndex).Take(PageSize).ToListAsync(cancellationToken: cancellationToken)
        };
    }

    public async Task<Page<TResult>> ListPagedAsync<TResult>(ISpecification<TEntity, TResult> specification, string? continuationToken, CancellationToken cancellationToken = default)
    {
        TryDecodePageIndex(continuationToken, out int pageIndex);

        var set = _context.Set<TEntity>().WithSpecification(specification);
        var pageCount = await set.CountAsync(cancellationToken: cancellationToken) / PageSize;
        var nexToken = pageIndex + 1 < pageCount ? EncodePageIndex(pageIndex + 1) : null;

        return new Page<TResult>
        {
            ContinuationToken = nexToken,
            Elements = await set.Skip(pageIndex).Take(PageSize).ToListAsync(cancellationToken: cancellationToken)
        };
    }
}
