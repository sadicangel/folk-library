//using FolkLibrary.Models;
//using System.Diagnostics;
//using System.Security.Cryptography;
//using System.Text;

//namespace FolkLibrary.IO;

//public static class AlbumReader
//{
//    private static readonly string[] Countries = new string[] { "Andorra", "Brasil", "Canada", "França", "Suiça", "USA", "Venezuela" };
//    private static readonly string[] Districts = new string[] { "Braga", "Lisboa", "Minho", "Montreal", "New Jersey", "Newark", "Paraná", "Paris", "Porto", "Rio de Janeiro", "São Paulo", "Viana do Castelo", "Yerres" };
//    private static readonly string[] Municipalities = new string[] { "Arcos de Valdevez", "Barcelos", "Caminha", "Esposende", "Famalicão", "Guimarães", "Lisboa", "Monção", "Paredes de Coura", "Ponte da Barca", "Ponte de Lima", "Santo Tirso", "Terras de Bouro", "Trofa", "Valença", "Viana do Castelo", "Vieira do Minho", "Vila Verde" };

//    private static readonly Dictionary<string, Artist> Artists = new();

//    private static readonly HashSet<Genre> Genres = new() { new Genre { Name = "Folk" } };

//    public static Album ReadOld(string folder)
//    {
//        var fileName = Path.GetFileName(folder);
//        if (fileName.StartsWith("zzzz"))
//            throw new AlbumReadException($"Could not read {fileName}: invalid folder layout");
//        var index = fileName.LastIndexOf('-');
//        if (index < 0)
//            throw new AlbumReadException($"Could not read {fileName}: no location");
//        var split = new string[] { fileName[..index].Trim(), fileName[(index + 1)..].Trim() };
//        var title = split[0];
//        var country = default(string);
//        var district = default(string);
//        var municipality = default(string);
//        if (split.Length > 1)
//        {
//            split = split[1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
//            foreach (var part in split)
//            {
//                if (Countries.Contains(part))
//                    country = part;
//                if (Districts.Contains(part))
//                    district = part;
//                if (Municipalities.Contains(part))
//                    municipality = part;
//            }
//        }
//        var album = new Album
//        {
//            Name = title,
//            Description = fileName,
//            Country = country ?? "Portugal",
//            District = district,
//            Municipality = municipality,
//            Artists = new(),
//            Genres = Genres,
//            Tracks = new(),
//        };
//        if (album.Name is null)
//            throw new AlbumReadException($"Could not read {fileName}: no title");
//        var trackCount = 0;
//        var artistNames = new HashSet<string>();
//        foreach (var file in Directory.EnumerateFiles(folder))
//        {
//            var tFile = default(TagLib.File)!;
//            try
//            {
//                tFile = TagLib.File.Create(file);
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine("Not a media file");
//                Debug.WriteLine(ex);
//                continue;
//            }
//            if (tFile.Properties.MediaTypes != TagLib.MediaTypes.Audio)
//                continue;

//            artistNames.UnionWith(tFile.Tag.AlbumArtists);
//            artistNames.UnionWith(tFile.Tag.Performers);
//            if (tFile.Tag.Year != 0 && tFile.Tag.Year < (album.Year ?? 0))
//                album.Year = (int)tFile.Tag.Year;
//            var track = new Track
//            {
//                Name = GetTrackName(tFile.Tag.Title, file),
//                Description = Path.GetFileName(file),
//                Number = GetTrackNumber(tFile.Tag.Track, file),
//                Album = album,
//                Duration = tFile.Properties.Duration,
//                Genres = Genres,
//                Artists = new(),
//            };
//            if (track.Name is null)
//                Console.WriteLine();
//            artistNames = GetArtistNames(artistNames, title);
//            track.Artists.UnionWith(artistNames.Select(n =>
//            {
//                if (!Artists.TryGetValue(n, out var artist))
//                    Artists[n] = artist = new Artist
//                    {
//                        Name = n,
//                        Country = album.Country,
//                        District = album.District,
//                        Municipality = album.Municipality,
//                        Parish = album.Parish,
//                    };
//                return artist;
//            }));

//            album.Tracks.Add(track);
//            ++trackCount;
//        }

//        album.Genres.UnionWith(album.Tracks.SelectMany(t => t.Genres));
//        album.Artists.UnionWith(album.Tracks.SelectMany(t => t.Artists));
//        album.Duration = album.Tracks.Aggregate(TimeSpan.Zero, (acc, cur) => acc + cur.Duration);
//        album.TrackCount = album.Tracks.Count;

//        return album;
//    }

//    private static string GetTrackName(string? name, string file)
//    {
//        var fileName = name is not null
//                       && !name.Contains("Faixa", StringComparison.InvariantCultureIgnoreCase)
//                       && !name.Contains("Piste", StringComparison.InvariantCultureIgnoreCase)
//                       && !name.Contains("Track", StringComparison.InvariantCultureIgnoreCase)
//            ? name
//            : Path.GetFileNameWithoutExtension(file);
//        int i = 0;
//        while (i < fileName.Length && (Char.IsDigit(fileName[i]) || Char.IsWhiteSpace(fileName[i])))
//            ++i;
//        return fileName[i..];
//    }

//    private static int GetTrackNumber(uint number, string file)
//    {
//        ReadOnlySpan<char> fileName = Path.GetFileNameWithoutExtension(file);
//        int i = 0;
//        while (i < fileName.Length && (Char.IsDigit(fileName[i]) || Char.IsWhiteSpace(fileName[i])))
//            ++i;
//        if (i > 0)
//        {
//            fileName = fileName[..i];
//            i = 0;
//            while (i < fileName.Length)
//            {
//                if (Char.IsWhiteSpace(fileName[i]))
//                {
//                    fileName = fileName[..i];
//                    break;
//                }
//                ++i;
//            }
                    


//            if (Int32.TryParse(fileName, out int pNumber))
//                return pNumber;
//            else
//                throw new AlbumReadException($"Error reading {fileName.ToString()}: invalid track number");
//        }
//        return (int)number;
//    }

//    public static HashSet<string> GetArtistNames(HashSet<string> artistNames, string title)
//    {
//        artistNames.RemoveWhere(n
//            => String.IsNullOrWhiteSpace(n)
//            || n.Contains("Desconhecido", StringComparison.InvariantCultureIgnoreCase)
//            || n.Contains("Unknown", StringComparison.InvariantCultureIgnoreCase)
//            || n.Contains("Inconnu", StringComparison.InvariantCultureIgnoreCase)
//            || n.Contains("Interpreten", StringComparison.InvariantCultureIgnoreCase));
//        if (artistNames.Count == 0)
//            return new HashSet<string> { title };
//        return artistNames;
//    }
//}