using Dapper;
using Npgsql;
using System.Security.Cryptography;

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
        var timestamp = await connection.ExecuteScalarAsync<DateTime>("SELECT now();");
        return CreateVersion7(new DateTimeOffset(timestamp, TimeSpan.Zero));
    }

    private static Guid CreateVersion7(DateTimeOffset timestamp)
    {
        var unixMilliseconds = timestamp.ToUnixTimeMilliseconds();
        if (unixMilliseconds < 0)
            throw new ArgumentOutOfRangeException(nameof(timestamp), timestamp, "The timestamp must be after 1 January 1970.");

        // "UUIDv7 values are created by allocating a Unix timestamp in milliseconds in the most significant 48 bits ..."
        var timeHigh = (uint)(unixMilliseconds >> 16);
        var timeLow = (ushort)unixMilliseconds;

        // "... and filling the remaining 74 bits, excluding the required version and variant bits, with random bits"
        Span<byte> bytes = stackalloc byte[10];
        RandomNumberGenerator.Fill(bytes);

        var randA = (ushort)(0x7000u | ((bytes[0] & 0xF) << 8) | bytes[1]);

        return new Guid(timeHigh, timeLow, randA, (byte)(bytes[2] & 0x3F | 0x80), bytes[3], bytes[4], bytes[5], bytes[6], bytes[7], bytes[8], bytes[9]);
    }
}
