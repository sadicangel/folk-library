using FolkLibrary.Albums;
using FolkLibrary.Interfaces;

namespace FolkLibrary.Artists;

public sealed class ArtistDocument : Document<ArtistId>, IMapFrom<Artist>
{
    public string ShortName { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string? District { get; set; }

    public string? Municipality { get; set; }

    public string? Parish { get; set; }

    public bool IsAbroad { get; set; }

    public int AlbumCount { get; set; }

    public List<AlbumDocument> Albums { get; init; } = new()!;
}