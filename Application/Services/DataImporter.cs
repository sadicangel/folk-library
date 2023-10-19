using Marten;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FolkLibrary.Services;

public interface IDataImporter
{
    Task ImportAsync(string? folderName = null);
}

internal sealed class DataImporter : IDataImporter
{
    private static readonly JsonSerializerOptions? JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly ILogger<DataImporter> _logger;
    private readonly IFileProvider _fileProvider;
    private readonly IDocumentSession _dbSession;
    private readonly IUuidProvider _uuidProvider;

    public DataImporter(ILogger<DataImporter> logger, IFileProvider fileProvider, IDocumentSession dbSession, IUuidProvider uuidProvider)
    {
        _logger = logger;
        _fileProvider = fileProvider;
        _dbSession = dbSession;
        _uuidProvider = uuidProvider;
    }

    private readonly record struct ArtistIdentifier(Guid ArtistId, string ArtistFolder);

    public async Task ImportAsync(string? folderName = null)
    {
        var dataFolder = folderName ?? _fileProvider.GetFileInfo("data").PhysicalPath;

        if (String.IsNullOrWhiteSpace(dataFolder))
            throw new FolkLibraryException("DataFolder not specified");

        if (!Directory.Exists(dataFolder))
            throw new DirectoryNotFoundException(dataFolder);

        // Create artists.
        var artistsByName = new Dictionary<string, ArtistIdentifier>();
        foreach (var artistFolder in Directory.EnumerateDirectories(dataFolder).SkipLast(1))
        {
            var artist = await CreateArtist(artistFolder, _uuidProvider);
            _dbSession.Events.StartStream(artist.ArtistId, artist);
            _logger.LogInformation("{artistName}", artist.Name);
            artistsByName[artist.Name] = new ArtistIdentifier(artist.ArtistId, artistFolder);
        }
        await _dbSession.SaveChangesAsync();

        // Create albums.
        foreach (var (artistName, (artistId, folder)) in artistsByName)
        {
            _logger.LogInformation("{artistName}", artistName);
            foreach (var albumFolder in Directory.EnumerateDirectories(folder))
            {
                var album = await CreateAlbum(albumFolder, isCompilation: false, _uuidProvider);

                foreach (var trackFile in Directory.EnumerateFiles(albumFolder))
                {
                    var track = await CreateTrack(trackFile, _uuidProvider, out var performers);

                    // Ensure it's the same artist for every track.
                    if (performers.Count != 1)
                        throw new FolkLibraryException($"Number of artists do not match for track {trackFile}");
                    if (!performers.TryGetValue(artistName, out _))
                        throw new FolkLibraryException($"Artist '{artistName}' does not match artist for track {trackFile}");

                    album.Tracks.Add(track);
                }

                await _dbSession.Events.AppendOptimistic(artistId, album);
                _logger.LogInformation("\t - {albumName}", album.Name);
                await _dbSession.SaveChangesAsync();
            }
        }

        const string variousArtists = "Vários Artistas";
        _logger.LogInformation("{artistName}", variousArtists);
        var albumTracksByArtistId = new Dictionary<Guid, Dictionary<Guid, List<int>>>();
        foreach (var albumFolder in Directory.EnumerateDirectories(Path.Combine(dataFolder, variousArtists)))
        {
            var album = await CreateAlbum(albumFolder, isCompilation: true, _uuidProvider);
            _logger.LogInformation("\t - {albumName}", album.Name);
            var albumArtists = new List<Guid>();
            var tracksByArtistId = albumTracksByArtistId[album.AlbumId] = new Dictionary<Guid, List<int>>();
            foreach (var trackFile in Directory.EnumerateFiles(albumFolder))
            {
                var track = await CreateTrack(trackFile, _uuidProvider, out var performers);
                foreach (var performer in performers)
                {
                    if (!artistsByName.TryGetValue(performer, out var artistIdentifier))
                        throw new FolkLibraryException($"Track {trackFile} has no matching artist");
                    albumArtists.Add(artistIdentifier.ArtistId);
                    //if (!tracksByArtistId.TryGetValue(artist.ArtistId, out var artistTracks))
                    //    tracksByArtistId[artist.ArtistId] = artistTracks = new List<int>();
                    //artistTracks.Add(track.Number);
                }
                album.Tracks.Add(track);
            }

            foreach (var albumArtistId in albumArtists)
                await _dbSession.Events.AppendOptimistic(albumArtistId, album);
            await _dbSession.SaveChangesAsync();
        }
    }

    private static async ValueTask<ArtistCreated> CreateArtist(string artistFolder, IUuidProvider uuidProvider)
    {
        var artistName = Path.GetFileName(artistFolder);
        var artistInfo = JsonNode.Parse(File.ReadAllBytes(Path.Combine(artistFolder, "info.json")));
        artistInfo!["genres"] = JsonArray.Parse("""["Folk"]"""u8);
        artistInfo!["id"] = JsonValue.Parse($"\"{await uuidProvider.ProvideUuidAsync()}\"");
        var artist = JsonSerializer.Deserialize<ArtistCreated>(artistInfo, JsonOptions)!;
        if (artist.Name != artistName)
            throw new FolkLibraryException($"Folder '{artistName}' != ArtistName {artist.Name}");
        if (string.IsNullOrEmpty(artist.ShortName))
            throw new FolkLibraryException($"Artist '{artistName}' does not have short name");
        return artist;
    }

    private static async ValueTask<AlbumCreated> CreateAlbum(string albumFolder, bool isCompilation, IUuidProvider uuidProvider)
    {
        var albumName = Path.GetFileName(albumFolder);

        return new AlbumCreated(
            AlbumId: await uuidProvider.ProvideUuidAsync(),
            Name: albumName,
            Description: null,
            Year: ParseYear(albumName),
            IsCompilation: isCompilation,
            Genres: new List<string> { "Folk" },
            Tracks: new List<Track>());

        static int? ParseYear(string albumName) => albumName.Length > 4 && int.TryParse(albumName.AsSpan()[^4..], out var albumYear) ? albumYear : null;
    }

    private static ValueTask<Track> CreateTrack(string trackFile, IUuidProvider uuidProvider, out HashSet<string> performers)
    {
        var meta = TagLib.File.Create(trackFile);
        var tag = meta.Tag;
        performers = new HashSet<string>(tag.Performers);

        return CreateTrackAsync(trackFile, meta, tag, uuidProvider);

        static async ValueTask<Track> CreateTrackAsync(string trackFile, TagLib.File meta, TagLib.Tag tag, IUuidProvider uuidProvider)
        {
            var track = new Track(
                TrackId: await uuidProvider.ProvideUuidAsync(),
                Name: tag.Title ?? throw new FolkLibraryException($"Track '{trackFile}' has no title"),
                Number: tag.Track is not 0 ? (int)tag.Track : throw new FolkLibraryException($"Track '{trackFile}' has no number"),
                Description: null,
                Year: tag.Year != 0 ? (int)tag.Year : null,
                IsYearUncertain: tag.Year == 0,
                Duration: meta.Properties.Duration,
                Genres: new List<string> { "Folk" }
            );

            return track;
        }
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