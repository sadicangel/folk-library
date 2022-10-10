namespace FolkLibrary.Exceptions;

public abstract class FolkLibraryException : Exception
{
	protected FolkLibraryException(string? message) : base(message)
	{

	}
}