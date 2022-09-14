namespace FolkLibrary.Exceptions;

public sealed class FolkDataLoadException : Exception
{
    public FolkDataLoadException(string? message) : base(message)
    {
    }
}
