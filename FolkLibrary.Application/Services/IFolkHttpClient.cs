using FolkLibrary.Artists;

namespace FolkLibrary.Interfaces;
public interface IFolkHttpClient
{
    Task<ArtistDto> GetArtistByIdAsync(Guid artistId);
    Task<Page<ArtistDto>> GetArtistsAsync(ArtistFilterDto? filter = null, string? continuationToken = null, int? pageSize = null);
}