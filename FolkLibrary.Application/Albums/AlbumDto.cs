using FolkLibrary.Application.Interfaces;
using FolkLibrary.Tracks;

namespace FolkLibrary.Albums;

public sealed class AlbumDto : IDocument, IMapFrom<Album>
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public string? Description { get; set; }

    public int? Year { get; set; }

    public bool IsYearUncertain { get; set; }

    public required HashSet<string> Genres { get; init; }

    public required int TrackCount { get; set; }

    public required TimeSpan Duration { get; set; }

    public bool IsIncomplete { get; set; }

    public required List<TrackDto> Tracks { get; init; }
}
