using Dapper;
using Npgsql;

namespace FolkLibrary.Services;

public interface IUuidProvider
{
    Task<Guid> ProvideUuidAsync(CancellationToken cancellationToken = default);
}

internal sealed class UuidProvider : IUuidProvider
{
    private readonly NpgsqlDataSource _dataSource;

    public UuidProvider(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<Guid> ProvideUuidAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<Guid>("SELECT gen_random_uuid()");
    }
}
