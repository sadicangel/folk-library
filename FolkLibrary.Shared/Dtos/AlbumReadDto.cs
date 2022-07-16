namespace FolkLibrary.Dtos;

public sealed class AlbumReadDto : ItemReadDto
{
    public int? Year { get; set; }
    public int TrackCount { get; set; }
    public TimeSpan Duration { get; set; }
    public HashSet<ItemReadDto> Artists { get; set; } = null!;
    public HashSet<ItemReadDto> Genres { get; set; } = null!;
    public List<ItemReadDto> Tracks { get; set; } = null!;
}