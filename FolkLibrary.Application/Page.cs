namespace FolkLibrary;

public readonly record struct Page<T>(IReadOnlyList<T> Items, string? ContinuationToken)
{
    public bool HasMoreResults { get => ContinuationToken is not null; }
    public string? ContinuationToken { get; init; } = ContinuationToken;
    public int ItemsCount { get => Items.Count; }
    public IReadOnlyList<T> Items { get; init; } = Items;
}
