using System.Diagnostics.CodeAnalysis;

namespace FolkLibrary.Models;
public sealed class Page<T>
{
    [MemberNotNullWhen(true, nameof(ContinuationToken))]
    public bool HasMoreResults { get => !String.IsNullOrWhiteSpace(ContinuationToken); }
    public string? ContinuationToken { get; init; }
    public IReadOnlyList<T> Elements { get; init; } = null!;
}
