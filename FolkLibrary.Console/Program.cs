using FolkLibrary.Models;
using FolkLibrary.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

static string Encode(ReadOnlySpan<byte> bytes)
{
    Span<char> chars = stackalloc char[24];
    if (Convert.TryToBase64Chars(bytes, chars, out _))
    {
        for (int i = 0; i < chars.Length; ++i)
        {
            if (chars[i] == '/')
                chars[i] = '_';
            else if (chars[i] == '+')
                chars[i] = '-';
        }
        return new string(chars[..22]);
    }
    throw new Exception("Failed to generated ID");
}

static byte[] Decode(ReadOnlySpan<char> chars)
{
    Span<char> buffer = stackalloc char[24];
    buffer[22] = '=';
    buffer[23] = '=';
    chars.CopyTo(buffer[..22]);
    for (int i = 0; i < buffer.Length; ++i)
    {
        if (buffer[i] == '_')
            buffer[i] = '/';
        else if (buffer[i] == '-')
            buffer[i] = '+';
    }
    Span<byte> bytes = stackalloc byte[16];
    Convert.TryFromBase64Chars(buffer, bytes, out _);
    return bytes.ToArray();
}

static Guid CreateId(params string[] values)
{
    return new Guid(MD5.HashData(Encoding.UTF8.GetBytes(String.Concat(values))));
}

var options = new DbContextOptionsBuilder()
    .UseSqlite(@"DataSource=D:\Development\folk-library\FolkLibrary\db.sqlite")
    .EnableSensitiveDataLogging()
    .Options;
using var context = new FolkLibraryContext(options);
context.Database.EnsureDeleted();
context.Database.EnsureCreated();

var folk = context.Add(new Genre { Id = CreateId("Folk"), Name = "Folk", Type = "Genre" }).Entity;

foreach (var artistFolder in Directory.EnumerateDirectories(@"D:\Music\Folk"))
{
    var artistName = Path.GetFileName(artistFolder);
    Console.WriteLine(artistName);
    var artistDesc = JsonNode.Parse(File.ReadAllText(Path.Combine(artistFolder, "description.json")))!;
    var artist = new Artist
    {
        Id = CreateId("Artist", artistName),
        Name = artistName,
        Type = "Artist",
        Country = (string)artistDesc["country"]!,
        District = (string?)artistDesc["district"],
        Municipality = (string?)artistDesc["municipality"],
        Parish = (string?)artistDesc["parish"],
        Albums = new(),
        Tracks = new(),
    };

    foreach (var albumFolder in Directory.EnumerateDirectories(artistFolder))
    {
        var albumName = Path.GetFileName(albumFolder);
        Console.WriteLine("\t{0}", albumName);
        var album = new Album
        {
            Id = CreateId("Album", artistName, albumName),
            Name = albumName,
            Type = "Album",
            Year = null,
            Description = null,
            Genres = new HashSet<Genre> { folk },
            Tracks = new()
        };

        foreach (var trackFile in Directory.EnumerateFiles(albumFolder))
        {
            var meta = TagLib.File.Create(trackFile);
            var tag = meta.Tag;
            var track = new Track
            {
                Id = CreateId("Track", artistName, albumName, tag.Title),
                Name = tag.Title,
                Type = "Track",
                Description = null,
                Number = (int)tag.Track,
                Year = tag.Year != 0 ? (int)tag.Year : null,
                Duration = meta.Properties.Duration,
                Genres = new HashSet<Genre> { folk },
            };

            artist.Tracks.Add(track);
            album.Tracks.Add(track);
            album.TrackCount++;
            album.Duration += track.Duration;
        }

        artist.Albums.Add(album);
    }
    Console.WriteLine();
    context.Artists.Add(artist);
}

context.SaveChanges();

Console.WriteLine();