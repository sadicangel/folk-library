using FolkLibrary.Artists;
using FolkLibrary.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;

namespace FolkLibrary.Services;
internal sealed class FolkHttpClient : IFolkHttpClient
{
    private readonly HttpClient _httpClient;

    public FolkHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Page<ArtistDto>> GetArtistsAsync(ArtistFilterDto? filter = null, string? continuationToken = null, int? pageSize = null)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"api/artist{filter.ToQueryParams()}", UriKind.Relative),
            Headers =
            {
                { "X-Continuation-Token", continuationToken },
                { "X-Page-Size", pageSize?.ToString() }
            }
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

file static class QueryParamsHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public static string ToQueryParams<T>(this T? obj) where T : class
    {
        if (obj is null)
            return String.Empty;

        var json = JsonSerializer.Serialize(obj, JsonOptions);
        var node = JsonNode.Parse(json);

        if (node is null)
            return String.Empty;

        var query = HttpUtility.ParseQueryString("");
        foreach (var (key, child) in node.AsObject())
        {
            if (child?.AsValue() is JsonValue value)
                query[key] = value.ToString();
        }

        return query.ToString()!;
    }
}
