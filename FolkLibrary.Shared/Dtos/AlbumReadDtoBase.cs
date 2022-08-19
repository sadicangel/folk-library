namespace FolkLibrary.Dtos;

public class AlbumReadDtoBase : ItemReadDto
{
    public int TrackCount { get; set; }
    public TimeSpan Duration { get; set; }
    public bool IsIncomplete { get; set; }
}
