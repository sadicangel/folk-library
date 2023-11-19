using DotNext;
using FolkLibrary.Albums;
using FolkLibrary.Artists;
using FolkLibrary.Tracks;
using MediatR;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FolkLibrary.Services;

public interface IDataImporter
{
    Task ImportAsync(string? folderName = null);
}

file readonly record struct ArtistIdentifier(Guid ArtistId, string ArtistFolder);

internal sealed class DataImporter(
    ILogger<DataImporter> logger,
    IMediator mediator,
    IFileProvider fileProvider) : IDataImporter
{
    private static readonly JsonSerializerOptions? JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task ImportAsync(string? folderName = null)
    {
        var dataFolder = folderName ?? fileProvider.GetFileInfo("data").PhysicalPath;

        if (String.IsNullOrWhiteSpace(dataFolder))
            throw new FolkLibraryException("DataFolder not specified");

        if (!Directory.Exists(dataFolder))
            throw new DirectoryNotFoundException(dataFolder);

        // Create artists.
        var artistsByName = new Dictionary<string, ArtistIdentifier>();
        foreach (var artistFolder in Directory.EnumerateDirectories(dataFolder).SkipLast(1))
        {
            var createArtist = CreateArtist(artistFolder);
            var artistId = await mediator.Send(createArtist).UnwrapAsync();
            logger.LogInformation("{artistName}", createArtist.Name);
            artistsByName[createArtist.Name] = new ArtistIdentifier(artistId, artistFolder);
        }

        // Create albums.
        foreach (var (artistName, (artistId, folder)) in artistsByName)
        {
            logger.LogInformation("{artistName}", artistName);
            var albumFolders = Directory.GetDirectories(folder);
            foreach (var albumFolder in albumFolders)
            {
                var album = CreateAlbum(albumFolder);
                var albumId = await mediator.Send(album).UnwrapAsync();

                logger.LogInformation("{marker} {albumName}",
                    albumFolder == albumFolders[^1] ? "└──" : "├──",
                    album.Name);

                var trackFiles = Directory.GetFiles(albumFolder);
                foreach (var trackFile in trackFiles)
                {
                    var track = CreateTrack(trackFile, out var performers);

                    logger.LogInformation("{marker1} {marker} {trackNumber:00} {trackName}",
                        albumFolder == albumFolders[^1] ? "    " : "│   ",
                        trackFile == trackFiles[^1] ? "└──" : "├──",
                        track.Number,
                        track.Name);

                    // Ensure it's the same artist for every track.
                    if (performers.Count != 1)
                        throw new FolkLibraryException($"Number of artists do not match for track {trackFile}");
                    if (!performers.TryGetValue(artistName, out var k))
                        throw new FolkLibraryException($"Artist '{artistName}' does not match artist for track {trackFile}");

                    var trackId = await mediator.Send(track).UnwrapAsync();
                    await mediator.Send(new AddAlbumTrack(albumId, trackId));
                }

                await mediator.Send(new AddArtistAlbum(artistId, albumId));
            }
        }

        const string variousArtists = "Vários Artistas";
        logger.LogInformation("{artistName}", variousArtists);
        //var albumTracksByArtistId = new Dictionary<Guid, Dictionary<Guid, List<int>>>();
        var variousAlbums = Directory.GetDirectories(Path.Combine(dataFolder, variousArtists));
        foreach (var albumFolder in variousAlbums)
        {
            var album = CreateAlbum(albumFolder);
            var albumId = await mediator.Send(album).UnwrapAsync();

            logger.LogInformation("{marker} {albumName}",
                albumFolder == variousAlbums[^1] ? "└──" : "├──",
                album.Name);

            var artistIds = new HashSet<Guid>();
            //var tracksByArtistId = albumTracksByArtistId[album.AlbumId] = new Dictionary<Guid, List<int>>();
            var trackFiles = Directory.GetFiles(albumFolder);
            foreach (var trackFile in trackFiles)
            {
                var track = CreateTrack(trackFile, out var performers);
                var trackId = await mediator.Send(track).UnwrapAsync();

                logger.LogInformation("{marker1} {marker} {trackNumber:00} {trackName}",
                    albumFolder == variousAlbums[^1] ? "    " : "│   ",
                    trackFile == trackFiles[^1] ? "└──" : "├──",
                    track.Number,
                    track.Name);

                foreach (var performer in performers)
                {
                    if (!artistsByName.TryGetValue(performer, out var artistIdentifier))
                        throw new FolkLibraryException($"Track {trackFile} has no matching artist");
                    artistIds.Add(artistIdentifier.ArtistId);
                    //if (!tracksByArtistId.TryGetValue(artist.ArtistId, out var artistTracks))
                    //    tracksByArtistId[artist.ArtistId] = artistTracks = new List<int>();
                    //artistTracks.Add(track.Number);
                }
                await mediator.Send(new AddAlbumTrack(albumId, trackId));
            }

            foreach (var artistId in artistIds)
                await mediator.Send(new AddArtistAlbum(artistId, albumId));
        }
    }

    private static CreateArtist CreateArtist(string artistFolder)
    {
        var artistName = Path.GetFileName(artistFolder);
        var infoJson = File.ReadAllBytes(Path.Combine(artistFolder, "info.json"));
        var artist = JsonSerializer.Deserialize<CreateArtist>(infoJson, JsonOptions)!;
        if (artist.Name != artistName)
            throw new FolkLibraryException($"Folder '{artistName}' != ArtistName {artist.Name}");
        if (string.IsNullOrEmpty(artist.ShortName))
            throw new FolkLibraryException($"Artist '{artistName}' does not have short name");
        return artist;
    }

    private static CreateAlbum CreateAlbum(string albumFolder)
    {
        var albumName = Path.GetFileName(albumFolder);

        return new CreateAlbum(
            Name: albumName,
            Description: null,
            Year: ParseYear(albumName));

        static int? ParseYear(string albumName) => albumName.Length > 4 && int.TryParse(albumName.AsSpan()[^4..], out var albumYear) ? albumYear : null;
    }

    private static CreateTrack CreateTrack(string trackFile, out HashSet<string> performers)
    {
        var meta = TagLib.File.Create(trackFile);
        var tag = meta.Tag;
        performers = new HashSet<string>(tag.Performers);

        var track = new CreateTrack(
            Name: tag.Title ?? throw new FolkLibraryException($"Track '{trackFile}' has no title"),
            Number: tag.Track is not 0 ? (int)tag.Track : throw new FolkLibraryException($"Track '{trackFile}' has no number"),
            Description: null,
            Year: tag.Year != 0 ? (int)tag.Year : null,
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