namespace FolkLibrary.Exceptions;

public sealed class UnknownException : FolkLibraryException
{
    public UnknownException(string? message) : base(message)
    {

    }
}
