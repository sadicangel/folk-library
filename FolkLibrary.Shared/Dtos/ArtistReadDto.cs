namespace FolkLibrary.Dtos;
public sealed class ArtistReadDto : ArtistReadDtoBase
{
    public List<AlbumReadDtoBase> Albums { get; set; } = null!;
}
