using FolkLibrary.Albums.Commands;
using FolkLibrary.Artists;
using FolkLibrary.Artists.Commands;
using FolkLibrary.Exceptions;
using FolkLibrary.Repositories;
using MediatR;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FolkLibrary.Services;

internal sealed class FolkDataLoader
{
    private static readonly JsonSerializerOptions? JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly ILogger<FolkDataLoader> _logger;
    private readonly ISender _mediator;
    private readonly IFileProvider _fileProvider;
    private readonly IArtistDocumentRepository _artistDocumentRepository;

    public FolkDataLoader(ILogger<FolkDataLoader> logger, ISender sender, IFileProvider fileProvider, IArtistDocumentRepository artistDocumentRepository)
    {
        _logger = logger;
        _mediator = sender;
        _fileProvider = fileProvider;
        _artistDocumentRepository = artistDocumentRepository;
    }

    private readonly record struct ArtistIdentifier(ArtistId Id, string Name, string Folder);

    public async Task LoadDataAsync(string folderName = "data")
    {
        var dataFolder = _fileProvider.GetFileInfo(folderName).PhysicalPath;

        if (String.IsNullOrWhiteSpace(dataFolder))
            throw new InvalidOperationException("DataFolder not specified");

        // Create artists.
        var artists = new Dictionary<string, ArtistIdentifier>();
        foreach (var artistFolder in Directory.EnumerateDirectories(dataFolder).SkipLast(1))
        {
            var artist = ReadArtist(artistFolder);
            _logger.LogInformation("{artistName}", artist.Name);
            var artistId = await _mediator.Send(artist);
            artists[artist.Name] = new(artistId, artist.Name, artistFolder);
        }

        var timeout = 100;
        while (await _artistDocumentRepository.CountAsync() != artists.Count)
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
                var album = ReadAlbum(albumFolder);
                _logger.LogInformation("\t - {albumName}", album.Name);
                var performers = new HashSet<string>();
                foreach (var trackFile in Directory.EnumerateFiles(albumFolder))
                {
                    var track = ReadTrack(trackFile, ref performers);

                    if (performers.Count != 1)
                        throw new FolkDataLoadException($"Number of artists do not match for track {trackFile}");
                    if (!performers.TryGetValue(artist.Name, out _))
                        throw new FolkDataLoadException($"Artist '{artist.Name}' does not match artist for track {trackFile}");
                    album.Tracks.Add(track);
                }
                var albumId = await _mediator.Send(album);

                await _mediator.Send(new AddAlbumCommand
                {
                    ArtistId = artist.Id,
                    AlbumId = albumId,
                });
            }
        }

        const string variousArtists = "Vários Artistas";
        _logger.LogInformation("{artistName}", variousArtists);
        foreach (var albumFolder in Directory.EnumerateDirectories(Path.Combine(dataFolder, variousArtists)))
        {
            var album = ReadAlbum(albumFolder);
            _logger.LogInformation("\t - {albumName}", album.Name);
            var performers = new HashSet<string>();
            var tracksContributed = new Dictionary<ArtistId, List<int>>();
            foreach (var trackFile in Directory.EnumerateFiles(albumFolder))
            {
                var track = ReadTrack(trackFile, ref performers);
                foreach (var performer in performers)
                {
                    if (!artists.TryGetValue(performer, out var artist))
                        throw new FolkDataLoadException($"Track {trackFile} has no matching artist");
                    if (!tracksContributed.TryGetValue(artist.Id, out var trackNumbers))
                        tracksContributed[artist.Id] = trackNumbers = new List<int>();
                    trackNumbers.Add(track.Number);
                }
                album.Tracks.Add(track);
            }
            if (tracksContributed.Count == 0)
                throw new FolkDataLoadException($"No artists for album {album.Name}");
            var albumId = await _mediator.Send(album);
            foreach (var (artistId, trackList) in tracksContributed)
            {
                await _mediator.Send(new AddAlbumCommand
                {
                    ArtistId = artistId,
                    AlbumId = albumId,
                    TracksContributed = trackList

                });
            }
        }
    }


    private static CreateArtistCommand ReadArtist(string artistFolder)
    {
        var artistName = Path.GetFileName(artistFolder);
        var artist = JsonSerializer.Deserialize<CreateArtistCommand>(File.ReadAllText(Path.Combine(artistFolder, "info.json")), JsonOptions)!;
        if (artist.Name != artistName)
            throw new FolkDataLoadException($"Folder '{artistName}' != ArtistName {artist.Name}");
        if (string.IsNullOrEmpty(artist.ShortName))
            throw new FolkDataLoadException($"Artist '{artistName}' does not have short name");
        artist.Genres.Add(Genre.Folk);
        return artist;
    }

    private static CreateAlbumCommand ReadAlbum(string albumFolder)
    {
        var albumName = Path.GetFileName(albumFolder);

        var album = new CreateAlbumCommand
        {
            Name = albumName,
            Year = null,
            Description = null,
            Genres = new HashSet<Genre> { Genre.Folk },
            Tracks = new()
        };
        if (albumName.Length > 4 && int.TryParse(albumName.AsSpan()[^4..], out var albumYear))
            album.Year = albumYear;
        else
            album.IsYearUncertain = true;

        return album;
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
            Genres = new HashSet<Genre> { Genre.Folk },
        };

        if (track.Name is null)
            throw new FolkDataLoadException($"Track '{trackFile}' has no title");
        if (track.Number == 0)
            throw new FolkDataLoadException($"Track '{trackFile}' has no number");

        performers.UnionWith(tag.Performers);

        return track;
    }
}
