namespace FolkLibrary.IO;

public sealed class AlbumReadException : Exception
{
    public AlbumReadException(string? message) : base(message)
    {
    }
}
