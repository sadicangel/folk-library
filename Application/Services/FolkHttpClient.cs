using FolkLibrary.Albums;
using FolkLibrary.Artists;
using FolkLibrary.Tracks;
using Refit;

namespace FolkLibrary.Services;

public interface IFolkHttpClient
{
    [Get("/api/artists/{artistId}")]
    Task<Artist> GetArtistAsync(Guid artistId, CancellationToken cancellationToken = default);

    [Get("/api/artists")]
    Task<GetArtistsResponse> GetArtistsAsync(
        string? name = null,
        string? countryCode = null,
        string? countryName = null,
        string? district = null,
        string? municipality = null,
        string? parish = null,
        int? year = null,
        int? afterYear = null,
        int? beforeYear = null,
        OrderBy? orderBy = null,
        CancellationToken cancellationToken = default);

    [Get("/api/albums/{albumId}")]
    Task<Album> GetAlbumAsync(Guid albumId, CancellationToken cancellationToken = default);

    [Get("/api/albums")]
    Task<GetAlbumsResponse> GetAlbumsAsync(
        string? name = null,
        int? year = null,
        int? afterYear = null,
        int? beforeYear = null,
        OrderBy? orderBy = null,
        CancellationToken cancellationToken = default);

    [Get("/api/tracks/{trackId}")]
    Task<Track> GetTrackAsync(Guid trackId, CancellationToken cancellationToken = default);

    [Get("/api/tracks")]
    Task<GetTracksResponse> GetTracksAsync(
        string? name = null,
        int? year = null,
        int? afterYear = null,
        int? beforeYear = null,
        TimeSpan? aboveDuration = null,
        TimeSpan? belowDuration = null,
        OrderBy? orderBy = null,
        CancellationToken cancellationToken = default);
}