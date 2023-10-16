using System.Collections.Immutable;

namespace FolkLibrary;

public sealed record class Album(
    Guid Id,
    string Name,
    string? Description,
    int? Year,
    bool IsYearUncertain,
    bool IsCompilation,
    bool IsIncomplete,
    TimeSpan Duration,
    ImmutableHashSet<string> Genres,
    ImmutableHashSet<Track> Tracks
);