using DotNext;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FolkLibrary.Services;

public interface IDataImporter
{
    Task ImportAsync(string? folderName = null);
}

file readonly record struct ArtistIdentifier(Guid ArtistId, string ArtistFolder);

internal sealed class DataImporter : IDataImporter
{
    private static readonly JsonSerializerOptions? JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataImporter> _logger;
    private readonly IFileProvider _fileProvider;

    public DataImporter(
        IServiceProvider serviceProvider,
        ILogger<DataImporter> logger,
        IFileProvider fileProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _fileProvider = fileProvider;
    }

    private async Task<TResponse> ValidateAndHandleAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<TRequest>>();
        var handler = _serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

        await validator.ValidateAndThrowAsync(request, cancellationToken);
        return await handler.Handle(request, cancellationToken);
    }

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
            var artistId = await ValidateAndHandleAsync<CreateArtistCommand, Result<Guid>>(artist, CancellationToken.None).UnwrapAsync();
            _logger.LogInformation("{artistName}", artist.Name);
            artistsByName[artist.Name] = new ArtistIdentifier(artistId, artistFolder);
        }

        // Create albums.
        foreach (var (artistName, (artistId, folder)) in artistsByName)
        {
            _logger.LogInformation("{artistName}", artistName);
            foreach (var albumFolder in Directory.EnumerateDirectories(folder))
            {
                var album = CreateAlbum(albumFolder);

                foreach (var trackFile in Directory.EnumerateFiles(albumFolder))
                {
                    var track = CreateTrack(trackFile, out var performers);

                    // Ensure it's the same artist for every track.
                    if (performers.Count != 1)
                        throw new FolkLibraryException($"Number of artists do not match for track {trackFile}");
                    if (!performers.TryGetValue(artistName, out var k))
                        throw new FolkLibraryException($"Artist '{artistName}' does not match artist for track {trackFile}");
                    album.Tracks.Add(track);
                }

                var albumId = await ValidateAndHandleAsync<CreateAlbumCommand, Result<Guid>>(album).UnwrapAsync();

                var addAlbumToArtist = new AddAlbumToArtistCommand(artistId, albumId);
                await ValidateAndHandleAsync<AddAlbumToArtistCommand, Result<Unit>>(addAlbumToArtist);

                _logger.LogInformation("\t - {albumName}", album.Name);
            }
        }

        const string variousArtists = "Vários Artistas";
        _logger.LogInformation("{artistName}", variousArtists);
        //var albumTracksByArtistId = new Dictionary<Guid, Dictionary<Guid, List<int>>>();
        foreach (var albumFolder in Directory.EnumerateDirectories(Path.Combine(dataFolder, variousArtists)))
        {
            var album = CreateAlbum(albumFolder);

            var artistIds = new List<Guid>();
            //var tracksByArtistId = albumTracksByArtistId[album.AlbumId] = new Dictionary<Guid, List<int>>();
            foreach (var trackFile in Directory.EnumerateFiles(albumFolder))
            {
                var track = CreateTrack(trackFile, out var performers);
                foreach (var performer in performers)
                {
                    if (!artistsByName.TryGetValue(performer, out var artistIdentifier))
                        throw new FolkLibraryException($"Track {trackFile} has no matching artist");
                    artistIds.Add(artistIdentifier.ArtistId);
                    //if (!tracksByArtistId.TryGetValue(artist.ArtistId, out var artistTracks))
                    //    tracksByArtistId[artist.ArtistId] = artistTracks = new List<int>();
                    //artistTracks.Add(track.Number);
                }
                album.Tracks.Add(track);
            }

            var albumId = await ValidateAndHandleAsync<CreateAlbumCommand, Result<Guid>>(album).UnwrapAsync();
            foreach (var artistId in artistIds)
                await ValidateAndHandleAsync<AddAlbumToArtistCommand, Result<Unit>>(new AddAlbumToArtistCommand(artistId, albumId));
            _logger.LogInformation("\t - {albumName}", album.Name);
        }
    }

    private static CreateArtistCommand CreateArtist(string artistFolder)
    {
        var artistName = Path.GetFileName(artistFolder);
        var infoJson = File.ReadAllBytes(Path.Combine(artistFolder, "info.json"));
        var artist = JsonSerializer.Deserialize<CreateArtistCommand>(infoJson, JsonOptions)!;
        if (artist.Name != artistName)
            throw new FolkLibraryException($"Folder '{artistName}' != ArtistName {artist.Name}");
        if (string.IsNullOrEmpty(artist.ShortName))
            throw new FolkLibraryException($"Artist '{artistName}' does not have short name");
        return artist;
    }

    private static CreateAlbumCommand CreateAlbum(string albumFolder)
    {
        var albumName = Path.GetFileName(albumFolder);

        return new CreateAlbumCommand(
            Name: albumName,
            Description: null,
            Year: ParseYear(albumName),
            Tracks: new List<Track>());

        static int? ParseYear(string albumName) => albumName.Length > 4 && int.TryParse(albumName.AsSpan()[^4..], out var albumYear) ? albumYear : null;
    }

    private static Track CreateTrack(string trackFile, out HashSet<string> performers)
    {
        var meta = TagLib.File.Create(trackFile);
        var tag = meta.Tag;
        performers = new HashSet<string>(tag.Performers);

        var track = new Track(
            TrackId: Guid.NewGuid(),
            Name: tag.Title ?? throw new FolkLibraryException($"Track '{trackFile}' has no title"),
            Number: tag.Track is not 0 ? (int)tag.Track : throw new FolkLibraryException($"Track '{trackFile}' has no number"),
            Description: null,
            Year: tag.Year != 0 ? (int)tag.Year : null,
            IsYearUncertain: tag.Year == 0,
            Duration: meta.Properties.Duration
        );

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