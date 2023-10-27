using Dapper;
using Npgsql;
using System.Runtime.CompilerServices;
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

    [SkipLocalsInit]
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

    [SkipLocalsInit]
    private static Guid CreateVersion5(Guid namespaceId, ReadOnlySpan<byte> name)
    {
        // see https://github.com/LogosBible/Logos.Utility/blob/master/src/Logos.Utility/GuidUtility.cs and
        // https://faithlife.codes/blog/2011/04/generating_a_deterministic_guid/ for the original version of this code

        // convert the namespace UUID to network order (step 3)
        Span<byte> buffer = name.Length < 500 ? stackalloc byte[16 + name.Length + 20] : new byte[16 + name.Length + 20];
        if (!namespaceId.TryWriteBytes(buffer))
            throw new InvalidOperationException("Failed to write Guid bytes to buffer");
        SwapByteOrder(buffer);

        // compute the hash of the namespace ID concatenated with the name (step 4)
        name.CopyTo(buffer[16..]);
        var success = SHA1.TryHashData(buffer[..^20], buffer[^20..], out var bytesWritten);
        if (!success || bytesWritten < 16)
            throw new InvalidOperationException("Failed to hash data");

        // most bytes from the hash are copied straight to the bytes of the new GUID (steps 5-7, 9, 11-12)
        var newGuid = buffer[^20..^4];

        // set the four most significant bits (bits 12 through 15) of the time_hi_and_version field to the appropriate 4-bit version number from Section 4.1.3 (step 8)
        newGuid[6] = (byte)((newGuid[6] & 0x0F) | (5 << 4));

        // set the two most significant bits (bits 6 and 7) of the clock_seq_hi_and_reserved to zero and one, respectively (step 10)
        newGuid[8] = (byte)((newGuid[8] & 0x3F) | 0x80);

        // convert the resulting UUID to local byte order (step 13)
        SwapByteOrder(newGuid);
        return new Guid(newGuid);

        // Converts a GUID (expressed as a byte array) to/from network order (MSB-first).
        static void SwapByteOrder(Span<byte> guid)
        {
            SwapBytes(guid, 0, 3);
            SwapBytes(guid, 1, 2);
            SwapBytes(guid, 4, 5);
            SwapBytes(guid, 6, 7);

            static void SwapBytes(Span<byte> guid, int left, int right)
            {
                ref var first = ref Unsafe.AsRef(guid[0]);
                (Unsafe.Add(ref first, right), Unsafe.Add(ref first, left)) = (Unsafe.Add(ref first, left), Unsafe.Add(ref first, right));
            }
        }
    }
}
