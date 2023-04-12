using HashidsNet;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace FolkLibrary.Services;

internal sealed class EncryptorProvider : IEncryptorProvider
{
    private readonly ConcurrentDictionary<string, IEncryptor> _services = new();

    public IEncryptor GetEncryptService(string key)
    {
        return _services.GetOrAdd(key, CreateEncrypter);

        static IEncryptor CreateEncrypter(string key) => new Encryptor(key);
    }
}

file sealed class Encryptor : IEncryptor
{
    private readonly IHashids _hasher;

    public Encryptor(string key)
    {
        _hasher = new Hashids(GetSalt(key), minHashLength: 16);
    }

    private static string GetSalt(string key)
    {
        ReadOnlySpan<byte> source = Encoding.UTF8.GetBytes(key);
        Span<byte> target = stackalloc byte[16];
        MD5.HashData(source, target);
        return Encoding.UTF8.GetString(target);
    }

    public int Decrypt(string text) => _hasher.DecodeSingle(text);

    public bool TryDecrypt(string? text, out int number) => _hasher.TryDecodeSingle(text, out number);

    public string Encrypt(int number) => _hasher.Encode(number);
}