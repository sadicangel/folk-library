namespace FolkLibrary.Dtos;

public sealed class TrackReadDto : TrackReadDtoBase
{
    public AlbumReadDtoBase Album { get; set; } = null!;
    public HashSet<ArtistReadDtoBase> Artists { get; set; } = null!;
}