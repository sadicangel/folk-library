namespace FolkLibrary.Dtos;

public sealed class TrackReadDto : ItemReadDto
{
    public int Number { get; set; }
    public TimeSpan Duration { get; set; }
    public int? Year { get; set; }
    public ItemReadDto? Album { get; set; } = null!;
    public HashSet<ItemReadDto> Artists { get; set; } = null!;
    public HashSet<ItemReadDto> Genres { get; set; } = null!;
}