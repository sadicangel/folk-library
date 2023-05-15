using FolkLibrary.Albums;

namespace FolkLibrary.Artists;

public sealed class ArtistDto : IDocument
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string ShortName { get; init; }

    public required string LetterAvatar { get; init; }

    public string? Description { get; init; }

    public int? Year { get; init; }

    public bool IsYearUncertain { get; init; }

    public required string YearString { get; init; }

    public required HashSet<string> Genres { get; init; }

    public required string Country { get; init; }

    public string? District { get; init; }

    public string? Municipality { get; init; }

    public string? Parish { get; init; }

    public required string Location { get; init; }

    public bool IsAbroad { get; init; }

    public required List<AlbumDto> Albums { get; init; }
}
