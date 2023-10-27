using FolkLibrary.Albums;
using FolkLibrary.Artists;
using FolkLibrary.Tracks;
using Microsoft.Extensions.Http.AutoClient;

namespace FolkLibrary.Services;

[AutoClient(nameof(IFolkHttpClient))]
public interface IFolkHttpClient
{
    [Get("/api/artists/{artistId}")]
    Task<Artist> GetArtistAsync(Guid artistId, CancellationToken cancellationToken = default);

    [Get("/api/artists")]
    Task<GetArtistsResponse> GetArtistsAsync(
        [Query] string? name = null,
        [Query] string? countryCode = null,
        [Query] string? countryName = null,
        [Query] string? district = null,
        [Query] string? municipality = null,
        [Query] string? parish = null,
        [Query] int? year = null,
        [Query] int? afterYear = null,
        [Query] int? beforeYear = null,
        [Query] OrderBy? orderBy = null,
        CancellationToken cancellationToken = default);

    [Get("/api/albums/{albumId}")]
    Task<Album> GetAlbumAsync(Guid albumId, CancellationToken cancellationToken = default);

    [Get("/api/albums")]
    Task<GetAlbumsResponse> GetAlbumsAsync(
        [Query] string? name = null,
        [Query] int? year = null,
        [Query] int? afterYear = null,
        [Query] int? beforeYear = null,
        [Query] OrderBy? orderBy = null,
        CancellationToken cancellationToken = default);

    [Get("/api/tracks/{trackId}")]
    Task<Track> GetTrackAsync(Guid trackId, CancellationToken cancellationToken = default);

    [Get("/api/tracks")]
    Task<GetTracksResponse> GetTracksAsync(
        [Query] string? name = null,
        [Query] int? year = null,
        [Query] int? afterYear = null,
        [Query] int? beforeYear = null,
        [Query] TimeSpan? aboveDuration = null,
        [Query] TimeSpan? belowDuration = null,
        [Query] OrderBy? orderBy = null,
        CancellationToken cancellationToken = default);
}