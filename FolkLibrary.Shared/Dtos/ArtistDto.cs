using FolkLibrary.Interfaces;
using FolkLibrary.Models;

namespace FolkLibrary.Dtos;

public sealed class ArtistDto : IDataTransterObject, IMapFrom<Artist>
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string ShortName { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Description { get; set; }

    public int? Year { get; set; }

    public bool IsYearUncertain { get; set; }

    public HashSet<Genre> Genres { get; set; } = new();

    public string Country { get; set; } = null!;

    public string? District { get; set; }

    public string? Municipality { get; set; }

    public string? Parish { get; set; }

    public bool IsAbroad { get; set; }

    public int AlbumCount { get; set; }

    public List<AlbumDto> Albums { get; set; } = null!;
}
