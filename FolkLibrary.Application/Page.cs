namespace FolkLibrary;

public readonly record struct Page<T>(int PageIndex, bool HasMoreResults, IReadOnlyList<T> Items)
{
    public int PageIndex { get; init; } = PageIndex;
    public bool HasMoreResults { get; init; } = HasMoreResults;
    public int ItemsCount { get => Items.Count; }
    public IReadOnlyList<T> Items { get; init; } = Items;
}
