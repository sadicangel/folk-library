using FolkLibrary.Artists;
using FolkLibrary.Artists.Queries;

namespace FolkLibrary.Interfaces;
public interface IFolkHttpClient
{
    Task<ArtistDocument> GetArtistByIdAsync(Guid artistId);
    Task<Page<ArtistDocument>> GetAllArtistsAsync(GetAllArtistsQueryParams? queryParams = null, string? continuationToken = null);
}