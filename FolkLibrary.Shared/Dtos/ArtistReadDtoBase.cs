namespace FolkLibrary.Dtos;

public class ArtistReadDtoBase : ItemReadDto
{
    public string ShortName { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string? District { get; set; }
    public string? Municipality { get; set; }
    public string? Parish { get; set; }
}
