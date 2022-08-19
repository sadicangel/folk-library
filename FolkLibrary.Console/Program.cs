using FolkLibrary.IO;
using FolkLibrary.Models;
using FolkLibrary.Services;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

static string GetSafeString(string @string) => Regex.Replace(@string, @"[\/?:*""><|]+", "", RegexOptions.Compiled);

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

//foreach (var artistF in Directory.EnumerateDirectories(@"D:\Music\Folk"))
//{
//    foreach (var albumF in Directory.EnumerateDirectories(artistF))
//    {
//        foreach (var trackF in Directory.EnumerateFiles(albumF))
//        {
//            var trackM = TagLib.File.Create(trackF);
//            if (Int32.TryParse(trackM.Tag.Title[..2], out _))
//                Console.WriteLine(trackF);
//        }
//    }
//}
//return;

//var config = new NAudio.Lame.LameConfig();
//foreach (var artistF in Directory.EnumerateDirectories(@"D:\Music\Folk"))
//{
//    foreach (var albumF in Directory.EnumerateDirectories(artistF))
//    {
//        if (Directory.EnumerateFiles(albumF, "*.wma").Any())
//        {
//            Console.WriteLine(albumF);
//            foreach (var wma in Directory.EnumerateFiles(albumF, "*.wma"))
//            {
//                var mp3 = Path.ChangeExtension(wma, "mp3");
//                {
//                    using var reader = new NAudio.Wave.AudioFileReader(wma);
//                    using var writer = new NAudio.Lame.LameMP3FileWriter(mp3, reader.WaveFormat, config);
//                    reader.CopyTo(writer);
//                }
//                var wmaM = TagLib.File.Create(wma);
//                var mp3M = TagLib.File.Create(mp3);
//                wmaM.Tag.CopyTo(mp3M.Tag, overwrite: true);
//                mp3M.Save();
//                File.Delete(wma);
//            }
//        }
//    }
//}
//return;

var options = new DbContextOptionsBuilder()
    .UseNpgsql("Host=localhost;Username=postgres;Password=postgres;Database=folklibrary;Include Error Detail=true")
    //.EnableSensitiveDataLogging()
    //.LogTo(Console.WriteLine)
    .Options;
using var context = new FolkLibraryContext(options);

//var xlsx = @"D:\Music\folk.xlsx";
//File.Delete(xlsx);
//ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
//using var package = new ExcelPackage(new FileInfo(xlsx));
//var artistsSheet = package.Workbook.Worksheets.Add("artists");

//artistsSheet.Cells[1, 1].Value = "Name";
//artistsSheet.Cells[1, 2].Value = "Country";
//artistsSheet.Cells[1, 3].Value = "District";
//artistsSheet.Cells[1, 4].Value = "Municipality";
//artistsSheet.Cells[1, 5].Value = "Parish";
//artistsSheet.Cells[1, 6].Value = "Year";
//artistsSheet.Cells[1, 7].Value = "# Albums";
//int row = 2;
//foreach (var artist in context.Artists.Include(a => a.Albums).OrderBy(a => a.Name))
//{
//    int col = 1;
//    artistsSheet.Cells[row, col++].Value = artist.Name;
//    artistsSheet.Cells[row, col++].Value = artist.Country;
//    artistsSheet.Cells[row, col++].Value = artist.District;
//    artistsSheet.Cells[row, col++].Value = artist.Municipality;
//    artistsSheet.Cells[row, col++].Value = artist.Parish;
//    artistsSheet.Cells[row, col++].Value = artist.IsYearUncertain ? $"{artist.Year}?" : artist.Year.ToString();
//    artistsSheet.Cells[row, col++].Value = artist.Albums.Count;
//    row++;
//}
//artistsSheet.Cells[artistsSheet.Dimension.Address].AutoFitColumns();

//var albumSheet = package.Workbook.Worksheets.Add("albums");

//albumSheet.Cells[1, 1].Value = "Name";
//albumSheet.Cells[1, 2].Value = "Year";
//albumSheet.Cells[1, 3].Value = "# Tracks";
//albumSheet.Cells[1, 4].Value = "Artists";
//row = 2;
//foreach (var album in context.Albums.Include(a => a.Artists).OrderBy(a => a.Name))
//{
//    int col = 1;
//    albumSheet.Cells[row, col++].Value = album.Name;
//    albumSheet.Cells[row, col++].Value = album.Year?.ToString() ?? String.Empty;
//    albumSheet.Cells[row, col++].Value = album.TrackCount;
//    foreach (var artist in album.Artists)
//        albumSheet.Cells[row, col++].Value = artist.Name;
//    row++;
//}
//albumSheet.Cells[albumSheet.Dimension.Address].AutoFitColumns();

//package.Save();
//return;

context.Database.EnsureDeleted();
context.Database.EnsureCreated();

var artistCount = 0;
var albumCount = 0;
var trackCount = 0;

var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
foreach (var artistFolder in Directory.EnumerateDirectories(@"D:\Music\Folk"))
{
    artistCount++;
    var artistName = Path.GetFileName(artistFolder);
    Console.WriteLine(artistName);
    var singleArtist = default(Artist);
    var artists = new HashSet<Artist>();
    if (artistName != "Vários Artistas")
    {
        singleArtist = JsonSerializer.Deserialize<Artist>(File.ReadAllText(Path.Combine(artistFolder, "info.json")), jsonOptions)!;
        if (singleArtist.Name != artistName)
            throw new AlbumReadException($"Folder '{artistName}' != ArtistName {singleArtist.Name}");
        if (String.IsNullOrEmpty(singleArtist.ShortName))
            throw new AlbumReadException($"Artist '{artistName}' does not have short name");
        context.Artists.Add(singleArtist);
        artists.Add(singleArtist);
    }

    foreach (var albumFolder in Directory.EnumerateDirectories(artistFolder))
    {
        artists.Clear();
        albumCount++;
        var albumName = Path.GetFileName(albumFolder);

        Console.WriteLine("\t{0}", albumName);
        var album = new Album
        {
            Name = albumName,
            Year = null,
            Description = null,
            Genres = new HashSet<Genre> { Genre.Folk },
            Tracks = new()
        };
        if (albumName.Length > 4 && Int32.TryParse(albumName.AsSpan()[^4..], out var albumYear))
            album.Year = albumYear;

        foreach (var trackFile in Directory.EnumerateFiles(albumFolder))
        {
            trackCount++;
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
                throw new AlbumReadException($"Track '{trackFile}' has no title");
            if (track.Number == 0)
                throw new AlbumReadException($"Track '{trackFile}' has no number");

            if (singleArtist is not null)
            {
                if (tag.Performers.Length != 1)
                    throw new AlbumReadException($"Track '{track.Name}' must have a single artist. It has {tag.Performers.Length}");
                if (tag.Performers[0] != singleArtist.Name)
                    throw new AlbumReadException($"Track artist '{tag.Performers[0]}' does not match artist '{singleArtist.Name}'");
                singleArtist.Tracks.Add(track);
            }
            else
            {
                var variousArtists = context.Artists.Local.Where(a => meta.Tag.Performers.Contains(a.Name)).ToList();
                if (variousArtists.Count == 0)
                    throw new AlbumReadException($"No artist for track {trackFile}");
                foreach (var artist in variousArtists)
                {
                    artist.Tracks.Add(track);
                    artists.Add(artist);
                }
            }

            album.Tracks.Add(track);
            album.TrackCount++;
            album.Duration += track.Duration;
        }

        album.IsIncomplete = album.TrackCount != album.Tracks.Max(t => t.Number);

        if (singleArtist is not null)
        {
            singleArtist.Albums.Add(album);
        }
        else
        {
            foreach (var artist in artists)
                artist.Albums.Add(album);
        }
    }
    Console.WriteLine();
}

context.SaveChanges();

Console.Write("Validating data... ");
var albumsWithoutArtist = context.Albums.Where(a => a.Artists.Count == 0).ToList();
if (albumsWithoutArtist.Count > 0)
    throw new AlbumReadException($"Albums without artist:\n{String.Join('\n', albumsWithoutArtist.Select(t => t.Name))}");
var tracksWithoutAlbum = context.Tracks.Where(t => t.Album == null).ToList();
if (tracksWithoutAlbum.Count > 0)
    throw new AlbumReadException($"Tracks without album:\n{String.Join('\n', tracksWithoutAlbum.Select(t => t.Name))}");
var tracksWithoutArtist = context.Tracks.Where(t => t.Artists.Count == 0).ToList();
if (tracksWithoutArtist.Count > 0)
    throw new AlbumReadException($"Tracks without artist:\n{String.Join('\n', tracksWithoutArtist.Select(t => t.Name))}");
Console.WriteLine("Done!");

Console.WriteLine($"Artists..: {artistCount}");
Console.WriteLine($"Albums...: {albumCount}");
Console.WriteLine($"Tracks...: {trackCount}");
Console.WriteLine();