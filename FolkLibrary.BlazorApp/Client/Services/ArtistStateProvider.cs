using FolkLibrary.Artists;
using FolkLibrary.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace FolkLibrary.Services;

internal sealed class ArtistStateProvider
{
    private const int PageSize = 20;
    private readonly HashSet<ArtistDto> _artists = new(new ByIdComparer());
    private readonly Dictionary<string, ArtistDto> _artistsById = new();
    private readonly SortedDictionary<int, IReadOnlyList<ArtistDto>> _artistsByPage = new();
    private readonly IFolkHttpClient _httpClient;

    public IReadOnlyCollection<ArtistDto> Artists { get => _artists; }

    public event Action? StateChanged;

    public ArtistStateProvider(IFolkHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public ArtistDto GetById(string id) => _artistsById[id];

    public async Task<IEnumerable<ArtistDto>> FetchPageAsync(int index, int size = PageSize)
    {
        if (!_artistsByPage.TryGetValue(index, out var list))
        {
            var page = await _httpClient.GetArtistsAsync(pageIndex: index, pageSize: size);
            _artistsByPage[index] = list = page.Items;
            AddRange(list);
        }
        return list;
    }

    public void Add(ArtistDto artist)
    {
        if (_artists.Add(artist))
        {
            _artistsById[artist.Id] = artist;
            StateChanged?.Invoke();
        }
    }

    public void AddRange(IEnumerable<ArtistDto> artists)
    {
        artists = artists.Where(_artists.Add);
        foreach (var artist in artists)
            _artistsById[artist.Id] = artist;
        if (artists.Any())
            StateChanged?.Invoke();
    }

    public void Remove(ArtistDto artist)
    {
        if (_artists.Remove(artist))
        {
            _artistsById.Remove(artist.Id);
            StateChanged?.Invoke();
        }
    }

    public void Replace(IEnumerable<ArtistDto> artists)
    {
        _artists.Clear();
        AddRange(artists);
    }
}

file sealed class ByIdComparer : IEqualityComparer<ArtistDto>
{
    public bool Equals(ArtistDto? x, ArtistDto? y) => x is null ? y is null : x.Id == y?.Id;
    public int GetHashCode([DisallowNull] ArtistDto obj) => obj.Id.GetHashCode();
}
