namespace FolkLibrary.Dtos;

public class ArtistReadDtoBase : ItemReadDto
{
    public int? Year { get; set; }
    public string Country { get; set; } = null!;
    public string? District { get; set; }
    public string? Municipality { get; set; }
    public string? Parish { get; set; }
}
