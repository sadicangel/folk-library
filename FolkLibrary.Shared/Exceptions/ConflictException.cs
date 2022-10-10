namespace FolkLibrary.Exceptions;

public sealed class ConflictException : FolkLibraryException
{
    public ConflictException(string? message) : base(message)
    {
    }
}
