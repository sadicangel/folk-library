using System.Diagnostics.CodeAnalysis;

namespace FolkLibrary;
public sealed class Page<T>
{
    [MemberNotNullWhen(true, nameof(ContinuationToken))]
    public bool HasMoreResults { get => !string.IsNullOrWhiteSpace(ContinuationToken); }
    public string? ContinuationToken { get; init; }
    public IReadOnlyList<T> Elements { get; init; } = null!;
}
