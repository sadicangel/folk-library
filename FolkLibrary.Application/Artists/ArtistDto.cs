using FolkLibrary.Albums;
using FolkLibrary.Application.Interfaces;

namespace FolkLibrary.Artists;

public sealed class ArtistDto : IDocument, IMapFrom<Artist>
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string ShortName { get; init; }

    public string? Description { get; set; }

    public int? Year { get; set; }

    public bool IsYearUncertain { get; set; }

    public required HashSet<string> Genres { get; init; }

    public required string Country { get; set; }

    public string? District { get; set; }

    public string? Municipality { get; set; }

    public string? Parish { get; set; }

    public bool IsAbroad { get; set; }

    public required int AlbumCount { get; set; }

    public required List<AlbumDto> Albums { get; init; }
}
