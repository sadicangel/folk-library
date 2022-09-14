using FolkLibrary.Dtos;

namespace FolkLibrary.Interfaces;
public interface IFolkHttpClient
{
    Task<AlbumReadDto> GetAlbumAsync(Guid albumId);
    Task<List<AlbumReadDto>> GetAlbumsAsync(Guid? artistId = null, Guid? genreId = null);
    Task<ArtistReadDto> GetArtistAsync(Guid artistId);
    Task<List<ArtistReadDto>> GetArtistsAsync(string? country = null, string? district = null, string? municipality = null, string? parish = null);
    Task<TrackReadDto> GetTrackAsync(Guid trackId);
    Task<List<TrackReadDto>> GetTracksAsync(Guid? albumId = null, Guid? artistId = null, Guid? genreId = null);
}