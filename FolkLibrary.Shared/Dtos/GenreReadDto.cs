namespace FolkLibrary.Dtos;

public sealed class GenreReadDto : ItemReadDto
{
    public List<ItemReadDto> Albums { get; set; } = null!;
}
