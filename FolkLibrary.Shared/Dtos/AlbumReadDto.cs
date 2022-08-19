namespace FolkLibrary.Dtos;

public sealed class AlbumReadDto : AlbumReadDtoBase
{
    public HashSet<ArtistReadDtoBase> Artists { get; set; } = null!;
    public List<TrackReadDtoBase> Tracks { get; set; } = null!;
}