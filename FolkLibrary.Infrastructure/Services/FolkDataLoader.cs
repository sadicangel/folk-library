using AutoMapper;
using FolkLibrary.Albums;
using FolkLibrary.Artists;
using FolkLibrary.Database;
using FolkLibrary.Repositories;
using FolkLibrary.Tracks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FolkLibrary.Services;

internal sealed class FolkDataLoader
{
    private static readonly JsonSerializerOptions? JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly ILogger<FolkDataLoader> _logger;
    private readonly IMapper _mapper;
    private readonly IFileProvider _fileProvider;
    private readonly FolkDbContext _dbContext;
    private readonly IArtistViewRepository _artistViewRepository;

    public FolkDataLoader(ILogger<FolkDataLoader> logger, IMapper mapper, IFileProvider fileProvider, FolkDbContext dbContext, IArtistViewRepository artistViewRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _fileProvider = fileProvider;
        _dbContext = dbContext;
        _artistViewRepository = artistViewRepository;
    }

    private readonly record struct ArtistIdentifier(Artist Artist, string Folder);

    public async Task LoadDataAsync(string folderName = "data")
    {
        var dataFolder = _fileProvider.GetFileInfo(folderName).PhysicalPath;

        if (String.IsNullOrWhiteSpace(dataFolder))
            throw new FolkLibraryException("DataFolder not specified");

        // Create artists.
        var artistsByName = new Dictionary<string, ArtistIdentifier>();
        foreach (var artistFolder in Directory.EnumerateDirectories(dataFolder).SkipLast(1))
        {
            var artist = ReadArtist(artistFolder);
            _logger.LogInformation("{artistName}", artist.Name);
            artistsByName[artist.Name] = new ArtistIdentifier(artist, artistFolder);
            _dbContext.Artists.Add(artist);
        }

        // Create albums.
        foreach (var (_, (artist, folder)) in artistsByName)
        {
            _logger.LogInformation("{artistName}", artist.Name);
            foreach (var albumFolder in Directory.EnumerateDirectories(folder))
            {
                var album = ReadAlbum(albumFolder);
                album.Artists.Add(artist);
                _logger.LogInformation("\t - {albumName}", album.Name);

                foreach (var trackFile in Directory.EnumerateFiles(albumFolder))
                {
                    var track = ReadTrack(trackFile, album, out var performers);

                    // Ensure it's the same artist for every track.
                    if (performers.Count != 1)
                        throw new FolkLibraryException($"Number of artists do not match for track {trackFile}");
                    if (!performers.TryGetValue(artist.Name, out _))
                        throw new FolkLibraryException($"Artist '{artist.Name}' does not match artist for track {trackFile}");
                    track.Artists.Add(artist);

                    album.Tracks.Add(track);
                }

                artist.Albums.Add(album);
            }
        }

        const string variousArtists = "Vários Artistas";
        _logger.LogInformation("{artistName}", variousArtists);
        foreach (var albumFolder in Directory.EnumerateDirectories(Path.Combine(dataFolder, variousArtists)))
        {
            var album = ReadAlbum(albumFolder);
            _logger.LogInformation("\t - {albumName}", album.Name);
            var albumArtists = new List<Artist>();
            foreach (var trackFile in Directory.EnumerateFiles(albumFolder))
            {
                var track = ReadTrack(trackFile, album, out var performers);
                foreach (var performer in performers)
                {
                    if (!artistsByName.TryGetValue(performer, out var artistIdentifier))
                        throw new FolkLibraryException($"Track {trackFile} has no matching artist");
                    var (artist, _) = artistIdentifier;
                    artist.Tracks.Add(track);
                    albumArtists.Add(artist);
                }
                album.Tracks.Add(track);
            }

            albumArtists.ForEach(a => a.Albums.Add(album));
        }

        await _dbContext.SaveChangesAsync();

        var artistDtos = _mapper.Map<IEnumerable<ArtistDto>>(artistsByName.Values.Select(a => a.Artist));
        await _artistViewRepository.AddRangeAsync(artistDtos);
    }

    private static Artist ReadArtist(string artistFolder)
    {
        var artistName = Path.GetFileName(artistFolder);
        var artistInfo = JsonNode.Parse(File.ReadAllBytes(Path.Combine(artistFolder, "info.json")));
        artistInfo!["genres"] = JsonArray.Parse("""["Folk"]"""u8);
        var artist = JsonSerializer.Deserialize<Artist>(artistInfo, JsonOptions)!;
        if (artist.Name != artistName)
            throw new FolkLibraryException($"Folder '{artistName}' != ArtistName {artist.Name}");
        if (string.IsNullOrEmpty(artist.ShortName))
            throw new FolkLibraryException($"Artist '{artistName}' does not have short name");
        return artist;
    }

    private static Album ReadAlbum(string albumFolder)
    {
        var albumName = Path.GetFileName(albumFolder);

        var year = ParseYear(albumName);

        var album = new Album
        {
            Name = albumName,
            Description = null,
            Year = year,
            IsYearUncertain = year is null,
            Genres = new HashSet<string> { "Folk" },
            Tracks = new(),
            Duration = TimeSpan.Zero
        };

        return album;

        static int? ParseYear(string albumName) => albumName.Length > 4 && int.TryParse(albumName.AsSpan()[^4..], out var albumYear) ? albumYear : null;
    }

    private static Track ReadTrack(string trackFile, Album album, out HashSet<string> performers)
    {
        performers = new HashSet<string>();

        var meta = TagLib.File.Create(trackFile);
        var tag = meta.Tag;

        var track = new Track
        {
            Album = album,
            Name = tag.Title,
            Description = null,
            Number = (int)tag.Track,
            Year = tag.Year != 0 ? (int)tag.Year : null,
            Duration = meta.Properties.Duration,
            Genres = new HashSet<string> { "Folk" },
        };

        if (track.Name is null)
            throw new FolkLibraryException($"Track '{trackFile}' has no title");
        if (track.Number == 0)
            throw new FolkLibraryException($"Track '{trackFile}' has no number");

        performers.UnionWith(tag.Performers);

        return track;
    }

    //private static readonly string[] Viras = new string[]
    //{
    //    "vira", "fandango", "tirana", "rosinha", "ritinha"
    //};

    //private static readonly string[] Gotas = new string[]
    //{
    //    "gota"
    //};

    //private static readonly string[] CaninhasVerdes = new string[]
    //{
    //    "cana verde", "caninha verde", "cana"
    //};

    //private static readonly string[] Chulas = new string[]
    //{
    //    "chula", "vareira"
    //};

    //private static List<string> GetTrackGenres(CreateTrackDto track)
    //{
    //    bool Contains(string genre) => track.Name.Contains(genre, StringComparison.InvariantCultureIgnoreCase);

    //    var genres = new List<string>();
    //    if (Viras.Any(Contains))
    //        genres.Add("Vira");

    //    if (Gotas.Any(Contains))
    //        genres.Add("Gota");

    //    if (CaninhasVerdes.Any(Contains))
    //        genres.Add("Caninha Verde");

    //    if (Chulas.Any(Contains))
    //        genres.Add("Chula");

    //    return genres;

    //}
}