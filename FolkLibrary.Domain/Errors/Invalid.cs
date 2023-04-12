namespace FolkLibrary.Errors;
public readonly struct Invalid
{
    public IDictionary<string, string[]> Errors { get; }

    public Invalid(IDictionary<string, string[]> errors)
    {
        Errors = errors;
    }
}
