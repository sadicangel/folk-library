namespace FolkLibrary.Dtos;

public class TrackReadDtoBase : ItemReadDto
{
    public int Number { get; set; }
    public TimeSpan Duration { get; set; }
    public int? Year { get; set; }
}
