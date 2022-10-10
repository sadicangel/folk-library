using FolkLibrary.Dtos;
using FolkLibrary.Models;

namespace FolkLibrary.Interfaces;
public interface IFolkHttpClient
{
    Task<ArtistDto> GetArtistByIdAsync(Guid artistId);
    Task<Page<ArtistDto>> GetArtistsAsync(int pageIndex, string? country = null, string? district = null, string? municipality = null, string? parish = null);
}