namespace FolkLibrary.Dtos;

public class AlbumReadDtoBase : ItemReadDto
{
    public int? Year { get; set; }
    public int TrackCount { get; set; }
    public TimeSpan Duration { get; set; }
}
