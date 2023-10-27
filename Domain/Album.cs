using Generator.Equals;

namespace FolkLibrary;

public sealed record class Album(
    Guid AlbumId,
    string Name,
    string? Description,
    int? Year,
    bool IsYearUncertain,
    bool IsIncomplete,
    TimeSpan Duration,
    [property: UnorderedEquality] List<Guid> Artists,
    [property: UnorderedEquality] List<Track> Tracks
)
{
    public bool IsCompilation { get => Artists is { Count: >= 2 }; }
}