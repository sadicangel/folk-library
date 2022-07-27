namespace FolkLibrary.Dtos;

public sealed class GenreReadDto : GenreReadDtoBase
{
    public List<AlbumReadDtoBase> Albums { get; set; } = null!;
}
