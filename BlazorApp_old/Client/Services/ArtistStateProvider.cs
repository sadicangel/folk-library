using System.Diagnostics.CodeAnalysis;

namespace FolkLibrary.Services;

internal sealed class ArtistStateProvider
{
    private const int PageSize = 20;
    private readonly HashSet<Artist> _artists = new(new ByIdComparer());
    private readonly Dictionary<Guid, Artist> _artistsById = new();
    private readonly SortedDictionary<int, IReadOnlyList<Artist>> _artistsByPage = new();
    private readonly IFolkHttpClient _httpClient;

    public IReadOnlyCollection<Artist> Artists { get => _artists; }

    public event Action? StateChanged;

    public ArtistStateProvider(IFolkHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Artist GetById(Guid id) => _artistsById[id];

    public async Task<IEnumerable<Artist>> FetchPageAsync(int index, int size = PageSize)
    {
        if (!_artistsByPage.TryGetValue(index, out var list))
        {
            var page = await _httpClient.GetArtistsAsync();
            _artistsByPage[index] = list = page.Artists;
            AddRange(list);
        }
        return list;
    }

    public void Add(Artist artist)
    {
        if (_artists.Add(artist))
        {
            _artistsById[artist.ArtistId] = artist;
            StateChanged?.Invoke();
        }
    }

    public void AddRange(IEnumerable<Artist> artists)
    {
        artists = artists.Where(_artists.Add);
        foreach (var artist in artists)
            _artistsById[artist.ArtistId] = artist;
        if (artists.Any())
            StateChanged?.Invoke();
    }

    public void Remove(Artist artist)
    {
        if (_artists.Remove(artist))
        {
            _artistsById.Remove(artist.ArtistId);
            StateChanged?.Invoke();
        }
    }

    public void Replace(IEnumerable<Artist> artists)
    {
        _artists.Clear();
        AddRange(artists);
    }
}

file sealed class ByIdComparer : IEqualityComparer<Artist>
{
    public bool Equals(Artist? x, Artist? y) => x is null ? y is null : x.ArtistId == y?.ArtistId;
    public int GetHashCode([DisallowNull] Artist obj) => obj.ArtistId.GetHashCode();
}
