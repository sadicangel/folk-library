using FolkLibrary.Albums.Commands;
using FolkLibrary.Application.Interfaces;
using FolkLibrary.Artists.Commands;
using FolkLibrary.Repositories;
using FolkLibrary.Tracks;
using MediatR;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FolkLibrary.Services;

internal sealed class FolkDataLoader
{
    private static readonly JsonSerializerOptions? JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly ILogger<FolkDataLoader> _logger;
    private readonly ISender _mediator;
    private readonly IFileProvider _fileProvider;
    private readonly IArtistViewRepository _artistViewRepository;

    public FolkDataLoader(ILogger<FolkDataLoader> logger, ISender sender, IFileProvider fileProvider, IArtistViewRepository artistViewRepository)
    {
        _logger = logger;
        _mediator = sender;
        _fileProvider = fileProvider;
        _artistViewRepository = artistViewRepository;
    }

    private readonly record struct ArtistIdentifier(string Id, string Name, string Folder);

    public async Task LoadDataAsync(string folderName = "data")
    {
        var dataFolder = _fileProvider.GetFileInfo(folderName).PhysicalPath;

        if (String.IsNullOrWhiteSpace(dataFolder))
            throw new FolkLibraryException("DataFolder not specified");

        // Create artists.
        var artists = new Dictionary<string, ArtistIdentifier>();
        foreach (var artistFolder in Directory.EnumerateDirectories(dataFolder).SkipLast(1))
        {
            var request = ReadArtist(artistFolder);
            _logger.LogInformation("{artistName}", request.Name);
            var response = await _mediator.Send(request);
            if (!response.TryPickT0(out var artist, out var errors))
                throw new FolkLibraryException($"Error importing artist {request.Name}: {errors.Value.GetType().Name}");
            artists[request.Name] = new ArtistIdentifier(artist.Id, artist.Name, artistFolder);
        }

        var timeout = 100;
        while (await _artistViewRepository.CountAsync() != artists.Count)
        {
            _logger.LogInformation("MongoDB not ready. Waiting {timeout} milliseconds to try again", timeout);
            await Task.Delay(timeout);
            timeout = (int)(timeout * 1.5);
        }

        foreach (var (_, artist) in artists)
        {
            _logger.LogInformation("{artistName}", artist.Name);
            foreach (var albumFolder in Directory.EnumerateDirectories(artist.Folder))
            {
                var request = ReadAlbum(albumFolder);
                _logger.LogInformation("\t - {albumName}", request.Name);
                var performers = new HashSet<string>();
                foreach (var trackFile in Directory.EnumerateFiles(albumFolder))
                {
                    var track = ReadTrack(trackFile, ref performers);

                    if (performers.Count != 1)
                        throw new FolkLibraryException($"Number of artists do not match for track {trackFile}");
                    if (!performers.TryGetValue(artist.Name, out _))
                        throw new FolkLibraryException($"Artist '{artist.Name}' does not match artist for track {trackFile}");
                    request.Tracks.Add(track);
                }
                var response = await _mediator.Send(request);
                if (!response.TryPickT0(out var album, out var errors))
                    throw new FolkLibraryException($"Error importing album {request.Name}: {errors.Value.GetType().Name}");

                await _mediator.Send(new AddArtistAlbumCommand
                {
                    ArtistId = artist.Id,
                    AlbumId = album.Id,
                });
            }
        }

        const string variousArtists = "Vários Artistas";
        _logger.LogInformation("{artistName}", variousArtists);
        foreach (var albumFolder in Directory.EnumerateDirectories(Path.Combine(dataFolder, variousArtists)))
        {
            var request = ReadAlbum(albumFolder);
            _logger.LogInformation("\t - {albumName}", request.Name);
            var performers = new HashSet<string>();
            var tracksContributed = new Dictionary<string, List<int>>();
            foreach (var trackFile in Directory.EnumerateFiles(albumFolder))
            {
                var track = ReadTrack(trackFile, ref performers);
                foreach (var performer in performers)
                {
                    if (!artists.TryGetValue(performer, out var artist))
                        throw new FolkLibraryException($"Track {trackFile} has no matching artist");
                    if (!tracksContributed.TryGetValue(artist.Id, out var trackNumbers))
                        tracksContributed[artist.Id] = trackNumbers = new List<int>();
                    trackNumbers.Add(track.Number);
                }
                request.Tracks.Add(track);
            }
            if (tracksContributed.Count == 0)
                throw new FolkLibraryException($"No artists for album {request.Name}");
            var response = await _mediator.Send(request);
            if (!response.TryPickT0(out var album, out var errors))
                throw new FolkLibraryException($"Error importing album {request.Name}: {errors.Value.GetType().Name}");
            foreach (var (artistId, trackList) in tracksContributed)
            {
                await _mediator.Send(new AddArtistAlbumCommand
                {
                    ArtistId = artistId,
                    AlbumId = album.Id,
                    TracksContributed = trackList

                });
            }
        }
    }

    private sealed class ArtistInfo : IMapTo<CreateArtistCommand>
    {
        public required string Name { get; init; }

        public required string ShortName { get; init; }

        public string? Description { get; init; }

        public int? Year { get; init; }

        public bool IsYearUncertain { get; init; }

        public required string Country { get; init; }

        public string? District { get; init; }

        public string? Municipality { get; init; }

        public string? Parish { get; init; }

        public bool IsAbroad { get; init; }
    }

    private static CreateArtistCommand ReadArtist(string artistFolder)
    {
        var artistName = Path.GetFileName(artistFolder);
        var artistInfo = JsonNode.Parse(File.ReadAllBytes(Path.Combine(artistFolder, "info.json")));
        artistInfo!["genres"] = JsonArray.Parse("""["Folk"]"""u8);
        var artist = JsonSerializer.Deserialize<CreateArtistCommand>(artistInfo, JsonOptions)!;
        if (artist.Name != artistName)
            throw new FolkLibraryException($"Folder '{artistName}' != ArtistName {artist.Name}");
        if (string.IsNullOrEmpty(artist.ShortName))
            throw new FolkLibraryException($"Artist '{artistName}' does not have short name");
        return artist;
    }

    private static CreateAlbumCommand ReadAlbum(string albumFolder)
    {
        var albumName = Path.GetFileName(albumFolder);

        var year = ParseYear(albumName);

        var album = new CreateAlbumCommand
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

    private static CreateTrackDto ReadTrack(string trackFile, ref HashSet<string> performers)
    {
        var meta = TagLib.File.Create(trackFile);
        var tag = meta.Tag;

        var track = new CreateTrackDto
        {
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
