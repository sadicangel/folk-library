using FolkLibrary.Dtos;
using FolkLibrary.Interfaces;
using FolkLibrary.Models;
using System.Net.Http.Json;
using System.Web;

namespace FolkLibrary.Services;
internal sealed class FolkHttpClient : IFolkHttpClient
{
    private readonly HttpClient _httpClient;

    public FolkHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Page<ArtistDto>> GetArtistsAsync(int pageIndex, string? country = null, string? district = null, string? municipality = null, string? parish = null)
    {
        var queryParams = HttpUtility.ParseQueryString($"{nameof(pageIndex)}={pageIndex}");
        if (country is not null)
            queryParams[nameof(country)] = country;
        if (district is not null)
            queryParams[nameof(district)] = district;
        if (municipality is not null)
            queryParams[nameof(municipality)] = municipality;
        if (parish is not null)
            queryParams[nameof(parish)] = parish;


        var uri = $"api/artist{queryParams.ToString()}";
        var response = await _httpClient.GetFromJsonAsync<Page<ArtistDto>>(uri);
        return response!;
    }

    public async Task<ArtistDto> GetArtistByIdAsync(Guid artistId)
    {
        var uri = $"api/artist/{artistId}";
        var response = await _httpClient.GetFromJsonAsync<ArtistDto>(uri);
        return response!;
    }
}
