namespace FolkLibrary.Exceptions;

public sealed class ForbiddenException : FolkLibraryException
{
    public ForbiddenException(string? message) : base(message)
    {
    }
}