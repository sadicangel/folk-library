using FolkLibrary.Artists;
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
        [Query] Sort? sort = null,
        CancellationToken cancellationToken = default);
}