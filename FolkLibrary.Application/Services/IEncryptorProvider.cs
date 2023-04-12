namespace FolkLibrary.Services;

public interface IEncryptorProvider
{
    IEncryptor GetEncryptService(string key);
}
