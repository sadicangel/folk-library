using FolkLibrary.Tracks;
using System.Diagnostics.CodeAnalysis;

namespace FolkLibrary.Albums;

public sealed class AlbumDto : IDocument
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    [MemberNotNullWhen(true, nameof(TracksContributedByArtist))]
    public bool IsCompilation { get; set; }

    public List<int>? TracksContributedByArtist { get; set; }

    public int? Year { get; init; }

    public bool IsYearUncertain { get; init; }

    public required string YearString { get; init; }

    public required List<string> Genres { get; init; }

    public required int TrackCount { get; init; }

    public required TimeSpan Duration { get; init; }

    public bool IsIncomplete { get; init; }

    public required List<TrackDto> Tracks { get; init; }
}
