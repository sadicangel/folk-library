using FolkLibrary.Dtos;
using FolkLibrary.Interfaces;
using FolkLibrary.Models;
using FolkLibrary.Queries.Artists;
using System.Net.Http.Json;

namespace FolkLibrary.Services;
internal sealed class FolkHttpClient : IFolkHttpClient
{
    private readonly HttpClient _httpClient;

    public FolkHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Page<ArtistDto>> GetAllArtistsAsync(GetAllArtistsQueryParams? queryParams = null, string? continuationToken = null)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"api/artist{queryParams?.ToQueryParams()}"),
            Headers = { { "X-Continuation-Token", continuationToken } }
        };
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Page<ArtistDto>>();
        return result!;
    }

    public async Task<ArtistDto> GetArtistByIdAsync(Guid artistId)
    {
        var uri = $"api/artist/{artistId}";
        var result = await _httpClient.GetFromJsonAsync<ArtistDto>(uri);
        return result!;
    }
}
