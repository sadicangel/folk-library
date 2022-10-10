namespace FolkLibrary.Exceptions;

public class NotFoundException : FolkLibraryException
{
    public NotFoundException(string? message) : base(message)
    {
    }
}