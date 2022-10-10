namespace FolkLibrary.Exceptions;

public sealed class FolkDataLoadException : FolkLibraryException
{
    public FolkDataLoadException(string? message) : base(message)
    {
    }
}
