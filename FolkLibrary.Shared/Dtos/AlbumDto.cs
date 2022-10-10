using FolkLibrary.Interfaces;
using FolkLibrary.Models;

namespace FolkLibrary.Dtos;

public sealed class AlbumDto : IDataTransterObject, IMapFrom<Album>
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Description { get; set; }

    public int? Year { get; set; }

    public bool IsYearUncertain { get; set; }

    public HashSet<Genre> Genres { get; set; } = new();

    public int TrackCount { get; set; }

    public TimeSpan Duration { get; set; }

    public bool IsIncomplete { get; set; }

    public List<TrackDto> Tracks { get; set; } = null!;
}
