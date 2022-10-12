using FolkLibrary.Dtos;
using FolkLibrary.Models;
using FolkLibrary.Queries.Artists;

namespace FolkLibrary.Interfaces;
public interface IFolkHttpClient
{
    Task<ArtistDto> GetArtistByIdAsync(Guid artistId);
    Task<Page<ArtistDto>> GetAllArtistsAsync(GetAllArtistsQueryParams? queryParams = null, string? continuationToken = null);
}