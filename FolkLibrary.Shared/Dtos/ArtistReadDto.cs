namespace FolkLibrary.Dtos;

public sealed class ArtistReadDto : ItemReadDto
{
    public int? Year { get; set; }
    public string Country { get; set; } = null!;
    public string? District { get; set; }
    public string? Municipality { get; set; }
    public string? Parish { get; set; }
    public List<ItemReadDto> Albums { get; set; } = null!;
}
