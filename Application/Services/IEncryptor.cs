namespace FolkLibrary.Services;

public interface IEncryptor
{
    public string Encrypt(int number);

    public int Decrypt(string text);

    public bool TryDecrypt(string? text, out int number);
}
