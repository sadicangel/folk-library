namespace FolkLibrary.Exceptions;

public sealed class UnauthorizedException : FolkLibraryException
{
    public UnauthorizedException(string? message) : base(message)
    {
    }
}
