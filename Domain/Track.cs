using System.Collections.Immutable;

namespace FolkLibrary;

public sealed record class Track(
    Guid Id,
    string Name,
    int Number,
    string? Description,
    int? Year,
    bool IsYearUncertain,
    TimeSpan Duration,
    ImmutableHashSet<string> Genres
);
