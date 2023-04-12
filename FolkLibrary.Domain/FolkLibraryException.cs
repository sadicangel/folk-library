using System.Runtime.Serialization;

namespace FolkLibrary;
public sealed class FolkLibraryException : InvalidOperationException
{
    public FolkLibraryException()
    {
    }

    public FolkLibraryException(string? message) : base(message)
    {
    }

    public FolkLibraryException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public FolkLibraryException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
