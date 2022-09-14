using FolkLibrary.Exceptions;
using FolkLibrary.Models;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FolkLibrary.Services;

internal sealed class FolkDataLoader
{
    private readonly ILogger<FolkDataLoader> _logger;
    private readonly IFileProvider _fileProvider;
    private readonly FolkDbContext _dbContext;
    private static readonly JsonSerializerOptions? JsonOptions = new(JsonSerializerDefaults.Web);

    public FolkDataLoader(ILogger<FolkDataLoader> logger, IFileProvider fileProvider, FolkDbContext dbContext)
    {
        _logger = logger;
        _fileProvider = fileProvider;
        _dbContext = dbContext;
    }

    public void LoadData(bool overwrite = false, string folderName = "data")
    {
        if (!_dbContext.Database.CanConnect() || overwrite)
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
            var dataFolder = _fileProvider.GetFileInfo(folderName).PhysicalPath;
            foreach (var artistFolder in Directory.EnumerateDirectories(dataFolder))
                LoadArtist(artistFolder);
            _dbContext.SaveChanges();
        }
    }

    public void ValidateData()
    {
        _logger.LogInformation("Validating data... ");
        var albumsWithoutArtist = _dbContext.Albums.Where(a => a.Artists.Count == 0).ToList();
        if (albumsWithoutArtist.Count > 0)
            throw new FolkDataLoadException($"Albums without artist:\n{string.Join('\n', albumsWithoutArtist.Select(t => t.Name))}");
        var tracksWithoutAlbum = _dbContext.Tracks.Where(t => t.Album == null).ToList();
        if (tracksWithoutAlbum.Count > 0)
            throw new FolkDataLoadException($"Tracks without album:\n{string.Join('\n', tracksWithoutAlbum.Select(t => t.Name))}");
        var tracksWithoutArtist = _dbContext.Tracks.Where(t => t.Artists.Count == 0).ToList();
        if (tracksWithoutArtist.Count > 0)
            throw new FolkDataLoadException($"Tracks without artist:\n{string.Join('\n', tracksWithoutArtist.Select(t => t.Name))}");
        _logger.LogInformation("Done!");

        _logger.LogInformation("Artists..: {artistCount}", _dbContext.Artists.Count());
        _logger.LogInformation("Albums...: {albumCount}", _dbContext.Albums.Count());
        _logger.LogInformation("Tracks...: {trackCount}", _dbContext.Tracks.Count());
    }

    private void LoadArtist(string artistFolder)
    {
        Func<string, string[], List<Artist>> findArtists = null!;
        var isSingleArtist = !artistFolder.EndsWith("Vários Artistas");
        if (isSingleArtist)
        {
            var artist = ReadArtist(artistFolder);
            _logger.LogInformation("{artistName}", artist.Name);
            _dbContext.Artists.Add(artist);
            var list = new List<Artist> { artist };
            findArtists = (trackFile, performers) =>
            {
                if (performers.Length != 1 || list[0].Name != performers[0])
                    throw new FolkDataLoadException($"Artist '{artist.Name}' does not match artist for track {trackFile}");
                return list;
            };
        }
        else
        {
            _logger.LogInformation("Vários Artistas");
            findArtists = (trackFile, performers) =>
            {
                var list = _dbContext.Artists.Local.Where(a => performers.Contains(a.Name)).ToList();
                if (list.Count != performers.Length)
                    throw new FolkDataLoadException($"Number of artists do not match for track {trackFile}");
                return list;
            };
        }

        foreach (var albumFolder in Directory.EnumerateDirectories(artistFolder))
        {
            var album = ReadAlbum(albumFolder);
            _logger.LogInformation("\t - {albumName}", album.Name);
            LoadAlbumTracks(album, Directory.EnumerateFiles(albumFolder), findArtists);
        }
    }

    private static void LoadAlbumTracks(Album album, IEnumerable<string> trackFiles, Func<string, string[], List<Artist>> findArtists)
    {
        var albumArtists = new HashSet<Artist>();
        foreach (var trackFile in trackFiles)
        {
            var track = ReadTrack(trackFile, out var performers);
            var variousArtists = findArtists.Invoke(trackFile, performers);
            if (variousArtists.Count != performers.Length)
                throw new FolkDataLoadException($"Number of artists do not match for track {trackFile}");
            foreach (var otherArtist in variousArtists)
            {
                otherArtist.Tracks.Add(track);
                albumArtists.Add(otherArtist);
            }

            album.Tracks.Add(track);
            album.TrackCount++;
            album.Duration += track.Duration;
        }

        album.IsIncomplete = album.TrackCount != album.Tracks.Max(t => t.Number);

        foreach (var albumArtist in albumArtists)
        {
            albumArtist.Albums.Add(album);
            albumArtist.AlbumCount++;
        }
    }

    private static Artist ReadArtist(string artistFolder)
    {
        var artistName = Path.GetFileName(artistFolder);
        var artist = JsonSerializer.Deserialize<Artist>(File.ReadAllText(Path.Combine(artistFolder, "info.json")), JsonOptions)!;
        if (artist.Name != artistName)
            throw new FolkDataLoadException($"Folder '{artistName}' != ArtistName {artist.Name}");
        if (string.IsNullOrEmpty(artist.ShortName))
            throw new FolkDataLoadException($"Artist '{artistName}' does not have short name");
        artist.Genres.Add(Genre.Folk);
        return artist;
    }

    private static Album ReadAlbum(string albumFolder)
    {
        var albumName = Path.GetFileName(albumFolder);

        var album = new Album
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

    private static Track ReadTrack(string trackFile, out string[] performers)
    {
        var meta = TagLib.File.Create(trackFile);
        var tag = meta.Tag;

        var track = new Track
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

        performers = tag.Performers;

        return track;
    }
}
