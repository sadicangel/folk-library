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

    public DataImporter(ILogger<DataImporter> logger, IFileProvider fileProvider, IDocumentSession dbSession)
    {
        _logger = logger;
        _fileProvider = fileProvider;
        _dbSession = dbSession;
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
            var artist = CreateArtist(artistFolder);
            _dbSession.Events.StartStream(artist.Id, artist);
            _logger.LogInformation("{artistName}", artist.Name);
            artistsByName[artist.Name] = new ArtistIdentifier(artist.Id, artistFolder);
        }
        await _dbSession.SaveChangesAsync();

        // Create albums.
        foreach (var (artistName, (artistId, folder)) in artistsByName)
        {
            _logger.LogInformation("{artistName}", artistName);
            foreach (var albumFolder in Directory.EnumerateDirectories(folder))
            {
                var album = CreateAlbum(albumFolder, isCompilation: false);

                foreach (var trackFile in Directory.EnumerateFiles(albumFolder))
                {
                    var track = CreateTrack(trackFile, out var performers);

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
            var album = CreateAlbum(albumFolder, isCompilation: true);
            _logger.LogInformation("\t - {albumName}", album.Name);
            var albumArtists = new List<Guid>();
            var tracksByArtistId = albumTracksByArtistId[album.Id] = new Dictionary<Guid, List<int>>();
            foreach (var trackFile in Directory.EnumerateFiles(albumFolder))
            {
                var track = CreateTrack(trackFile, out var performers);
                foreach (var performer in performers)
                {
                    if (!artistsByName.TryGetValue(performer, out var artistIdentifier))
                        throw new FolkLibraryException($"Track {trackFile} has no matching artist");
                    albumArtists.Add(artistIdentifier.ArtistId);
                    //if (!tracksByArtistId.TryGetValue(artist.Id, out var artistTracks))
                    //    tracksByArtistId[artist.Id] = artistTracks = new List<int>();
                    //artistTracks.Add(track.Number);
                }
                album.Tracks.Add(track);
            }

            foreach (var albumArtistId in albumArtists)
                await _dbSession.Events.AppendOptimistic(albumArtistId, album);
            await _dbSession.SaveChangesAsync();
        }
    }

    private static ArtistCreated CreateArtist(string artistFolder)
    {
        var artistName = Path.GetFileName(artistFolder);
        var artistInfo = JsonNode.Parse(File.ReadAllBytes(Path.Combine(artistFolder, "info.json")));
        artistInfo!["genres"] = JsonArray.Parse("""["Folk"]"""u8);
        artistInfo!["id"] = JsonValue.Parse($"\"{Guid.NewGuid()}\"");
        var artist = JsonSerializer.Deserialize<ArtistCreated>(artistInfo, JsonOptions)!;
        if (artist.Name != artistName)
            throw new FolkLibraryException($"Folder '{artistName}' != ArtistName {artist.Name}");
        if (string.IsNullOrEmpty(artist.ShortName))
            throw new FolkLibraryException($"Artist '{artistName}' does not have short name");
        return artist;
    }

    private static AlbumCreated CreateAlbum(string albumFolder, bool isCompilation)
    {
        var albumName = Path.GetFileName(albumFolder);

        return new AlbumCreated(
            Id: Guid.NewGuid(),
            Name: albumName,
            Description: null,
            Year: ParseYear(albumName),
            IsCompilation: isCompilation,
            Genres: new List<string> { "Folk" },
            Tracks: new List<Track>());

        static int? ParseYear(string albumName) => albumName.Length > 4 && int.TryParse(albumName.AsSpan()[^4..], out var albumYear) ? albumYear : null;
    }

    private static Track CreateTrack(string trackFile, out HashSet<string> performers)
    {
        performers = new HashSet<string>();

        var meta = TagLib.File.Create(trackFile);
        var tag = meta.Tag;

        var track = new Track(
            Id: Guid.NewGuid(),
            Name: tag.Title ?? throw new FolkLibraryException($"Track '{trackFile}' has no title"),
            Number: tag.Track is not 0 ? (int)tag.Track : throw new FolkLibraryException($"Track '{trackFile}' has no number"),
            Description: null,
            Year: tag.Year != 0 ? (int)tag.Year : null,
            IsYearUncertain: tag.Year == 0,
            Duration: meta.Properties.Duration,
            Genres: new List<string> { "Folk" }
        );

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