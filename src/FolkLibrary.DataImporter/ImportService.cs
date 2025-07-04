using System.Text.Json;
using FolkLibrary.Domain;
using FolkLibrary.Domain.Albums;
using FolkLibrary.Domain.Artists;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FolkLibrary.DataImporter;

file readonly record struct ArtistIdentifier(Artist Artist, string ArtistFolder);

internal sealed class ImportService(IDocumentSession documentSession, IConfiguration configuration, ILogger<ImportService> logger)
{
    private static readonly JsonSerializerOptions? s_jsonOptions = new(JsonSerializerDefaults.Web);

    public async Task ImportAsync(CancellationToken cancellationToken = default)
    {
        var dataFolder = configuration.GetSection("FolkLibrary:DataFolder").Value;
        if (string.IsNullOrEmpty(dataFolder))
            throw new InvalidOperationException($"No data folder specified in configuration. Expected 'FolkLibrary:DataFolder' to specify a valid folder");

        // Create artists.
        var artists = new List<Artist>();
        var artistsByName = new Dictionary<string, ArtistIdentifier>();
        foreach (var artistFolder in Directory.EnumerateDirectories(dataFolder).SkipLast(1))
        {
            var artist = CreateArtist(artistFolder);
            artists.Add(artist);
            artistsByName[artist.Name] = new ArtistIdentifier(artist, artistFolder);
            logger.LogInformation("{artistName}", artist.Name);
        }

        // Create albums.v
        var albums = new List<Album>();
        foreach (var (artistName, (artist, folder)) in artistsByName)
        {
            logger.LogInformation("{artistName}", artistName);
            var albumFolders = Directory.GetDirectories(folder);
            foreach (var albumFolder in albumFolders)
            {
                var album = CreateAlbum(albumFolder);
                albums.Add(album);
                logger.LogInformation("{marker} {albumName}", albumFolder == albumFolders[^1] ? "└──" : "├──", album.Name);

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
                        throw new InvalidOperationException($"Number of artists do not match for track {trackFile}");
                    if (!performers.TryGetValue(artistName, out var k))
                        throw new InvalidOperationException($"Artist '{artistName}' does not match artist for track {trackFile}");

                    album.AddTrack(track);
                }

                album.LinkArtist(artist.Id);
                artist.LinkAlbum(album.Id);
            }
        }

        logger.LogInformation("{artistName}", "Vários Artistas");
        var variousAlbums = Directory.GetDirectories(Path.Combine(dataFolder, "Vários Artistas"));
        foreach (var albumFolder in variousAlbums)
        {
            var album = CreateAlbum(albumFolder);

            logger.LogInformation("{marker} {albumName}",
                albumFolder == variousAlbums[^1] ? "└──" : "├──",
                album.Name);

            var albumArtists = new HashSet<Artist>(EqualityComparer<Artist>.Create((a, b) => a is null ? b is null : a.Id == b?.Id, o => o.Id.GetHashCode()));
            var trackFiles = Directory.GetFiles(albumFolder);
            foreach (var trackFile in trackFiles)
            {
                var track = CreateTrack(trackFile, out var performers);

                logger.LogInformation("{marker1} {marker} {trackNumber:00} {trackName}",
                    albumFolder == variousAlbums[^1] ? "    " : "│   ",
                    trackFile == trackFiles[^1] ? "└──" : "├──",
                    track.Number,
                    track.Name);

                foreach (var performer in performers)
                {
                    if (!artistsByName.TryGetValue(performer, out var artistIdentifier))
                        throw new InvalidOperationException($"Track {trackFile} has no matching artist");
                    albumArtists.Add(artistIdentifier.Artist);
                }

                album.AddTrack(track);
            }

            foreach (var artist in albumArtists)
            {
                album.LinkArtist(artist.Id);
                artist.LinkAlbum(album.Id);
            }
        }

        foreach (var album in albums)
        {
            await documentSession.QueueChangesAsync(album, cancellationToken);
        }

        await documentSession.SaveChangesAsync(cancellationToken);

        foreach (var artist in artists)
        {
            await documentSession.QueueChangesAsync(artist, cancellationToken);
        }

        await documentSession.SaveChangesAsync(cancellationToken);
    }

    private readonly record struct ArtistInfo(
        string Name,
        string ShortName,
        string? Description,
        int? Year,
        bool IsYearUncertain,
        Location Location);

    private static Artist CreateArtist(string artistFolder)
    {
        var name = Path.GetFileName(artistFolder);
        var info = JsonSerializer.Deserialize<ArtistInfo>(File.ReadAllBytes(Path.Combine(artistFolder, "info.json")), s_jsonOptions)!;
        if (info.Name != name)
            throw new InvalidOperationException($"Folder '{name}' != ArtistName {info.Name}");
        if (string.IsNullOrEmpty(info.ShortName))
            throw new InvalidOperationException($"Artist '{name}' does not have short name");

        return new Artist(id: Guid.CreateVersion7(), info.Name, info.ShortName, info.Description, info.Year, info.IsYearUncertain, info.Location);
    }

    private static Album CreateAlbum(string albumFolder)
    {
        var albumName = Path.GetFileName(albumFolder);

        return new Album(id: Guid.CreateVersion7(), albumName, description: null, ParseYear(albumName));

        static int? ParseYear(string albumName) => albumName.Length > 4 && int.TryParse(albumName.AsSpan()[^4..], out var albumYear) ? albumYear : null;
    }

    private static Track CreateTrack(string trackFile, out HashSet<string> performers)
    {
        var meta = TagLib.File.Create(trackFile);
        var tag = meta.Tag;
        performers = [.. tag.Performers];

        if (string.IsNullOrEmpty(tag.Title))
            throw new InvalidOperationException($"Track '{trackFile}' has no title");

        if (tag.Track is not > 0)
            throw new InvalidOperationException($"Track '{trackFile}' has no number");

        var year = tag.Year != 0 ? (int?)tag.Year : null;

        var track = new Track(
            Name: tag.Title,
            Number: (int)tag.Track,
            Description: null,
            Year: year,
            IsYearUncertain: year is null,
            Duration: meta.Properties.Duration);

        return track;
    }
}
