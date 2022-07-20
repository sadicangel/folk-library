using FolkLibrary.Dtos;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;

namespace FolkLibrary.Services;
public sealed class FolkLibraryClient
{
    private readonly HttpClient _httpClient;

    public FolkLibraryClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<AlbumReadDto>> GetAlbumsAsync(Guid? artistId = null, Guid? genreId = null)
    {
        var queryParams = new Dictionary<string, string>();
        if (artistId is not null)
            queryParams[nameof(artistId)] = artistId.Value.ToString();
        if (genreId is not null)
            queryParams[nameof(genreId)] = genreId.Value.ToString();

        var uri = QueryHelpers.AddQueryString("api/album", queryParams);

        var response = await _httpClient.GetFromJsonAsync<List<AlbumReadDto>>(uri);
        return response!;
    }

    public async Task<AlbumReadDto> GetAlbumAsync(Guid albumId)
    {
        var uri = $"api/album/{albumId}";
        var response = await _httpClient.GetFromJsonAsync<AlbumReadDto>(uri);
        return response!;
    }

    public async Task<List<ArtistReadDto>> GetArtistsAsync(string? country = null, string? district = null, string? municipality = null, string? parish = null)
    {
        var queryParams = new Dictionary<string, string>();
        if (country is not null)
            queryParams[nameof(country)] = country;
        if (district is not null)
            queryParams[nameof(district)] = district;
        if (municipality is not null)
            queryParams[nameof(municipality)] = municipality;
        if (parish is not null)
            queryParams[nameof(parish)] = parish;

        var uri = QueryHelpers.AddQueryString("api/artist", queryParams);
        var response = await _httpClient.GetFromJsonAsync<List<ArtistReadDto>>(uri);
        return response!;
    }

    public async Task<ArtistReadDto> GetArtistAsync(Guid artistId)
    {
        var uri = $"api/artist/{artistId}";
        var response = await _httpClient.GetFromJsonAsync<ArtistReadDto>(uri);
        return response!;
    }

    public async Task<List<TrackReadDto>> GetTracksAsync(Guid? albumId = null, Guid? artistId = null, Guid? genreId = null)
    {
        var queryParams = new Dictionary<string, string>();
        if (albumId is not null)
            queryParams[nameof(albumId)] = albumId.Value.ToString();
        if (artistId is not null)
            queryParams[nameof(artistId)] = artistId.Value.ToString();
        if (genreId is not null)
            queryParams[nameof(genreId)] = genreId.Value.ToString();

        var uri = QueryHelpers.AddQueryString("api/track", queryParams);

        var response = await _httpClient.GetFromJsonAsync<List<TrackReadDto>>(uri);
        return response!;
    }

    public async Task<TrackReadDto> GetTrackAsync(Guid trackId)
    {
        var uri = $"api/track/{trackId}";
        var response = await _httpClient.GetFromJsonAsync<TrackReadDto>(uri);
        return response!;
    }
}
