using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FolkLibrary.Database;
using FolkLibrary.Services;
using Microsoft.EntityFrameworkCore;

namespace FolkLibrary.Repositories;

internal abstract class PostgresRepository<TEntity> : RepositoryBase<TEntity>, IRepository<TEntity> where TEntity : class
{
    private readonly FolkDbContext _context;
    private readonly IEncryptor _encryptor;

    public string CollectionName { get; }

    public PostgresRepository(FolkDbContext context, IEncryptorProvider encryptorProvider) : base(context)
    {
        CollectionName = context.Model.FindEntityType(typeof(TEntity))!.GetTableName()!;
        _context = context;
        _encryptor = encryptorProvider.GetEncryptService(CollectionName);
    }

    public async Task<Page<TEntity>> ListAsync(ISpecification<TEntity> specification, string? continuationToken, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        _encryptor.TryDecrypt(continuationToken, out int pageIndex);

        var set = _context.Set<TEntity>().WithSpecification(specification);
        var pageCount = await set.CountAsync(cancellationToken) / pageSize;
        var nexToken = pageIndex + 1 < pageCount ? _encryptor.Encrypt(pageIndex + 1) : null;

        return new Page<TEntity>
        {
            ContinuationToken = nexToken,
            Items = await set.Skip(pageIndex).Take(pageSize).ToListAsync(cancellationToken)
        };
    }

    public async Task<Page<TResult>> ListAsync<TResult>(ISpecification<TEntity, TResult> specification, string? continuationToken, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        _encryptor.TryDecrypt(continuationToken, out int pageIndex);

        var set = _context.Set<TEntity>().WithSpecification(specification);
        var pageCount = await set.CountAsync(cancellationToken) / pageSize;
        var nexToken = pageIndex + 1 < pageCount ? _encryptor.Encrypt(pageIndex + 1) : null;

        return new Page<TResult>
        {
            ContinuationToken = nexToken,
            Items = await set.Skip(pageIndex).Take(pageSize).ToListAsync(cancellationToken)
        };
    }
}
