using FolkLibrary.Artists;
using FolkLibrary.Artists.Queries;
using FolkLibrary.Interfaces;
using System.Net.Http.Json;

namespace FolkLibrary.Services;
internal sealed class FolkHttpClient : IFolkHttpClient
{
    private readonly HttpClient _httpClient;

    public FolkHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Page<ArtistDocument>> GetAllArtistsAsync(GetAllArtistsQueryParams? queryParams = null, string? continuationToken = null)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"api/artist{queryParams?.ToQueryParams()}"),
            Headers = { { "X-Continuation-Token", continuationToken } }
        };
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Page<ArtistDocument>>();
        return result!;
    }

    public async Task<ArtistDocument> GetArtistByIdAsync(Guid artistId)
    {
        var uri = $"api/artist/{artistId}";
        var result = await _httpClient.GetFromJsonAsync<ArtistDocument>(uri);
        return result!;
    }
}
