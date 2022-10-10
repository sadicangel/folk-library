namespace FolkLibrary.Models;
public sealed class Page<T>
{
    public bool HasMoreResults { get => !String.IsNullOrWhiteSpace(ContinuationToken); }
    public string? ContinuationToken { get; init; }
    public IReadOnlyList<T> Elements { get; init; } = null!;
}
