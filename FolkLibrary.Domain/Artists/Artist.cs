using FolkLibrary.Albums;
using FolkLibrary.Tracks;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FolkLibrary.Artists;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Artist : Entity
{
    public required string ShortName { get; set; }

    [StringLength(maximumLength: 2, MinimumLength = 2)]
    public required string Country { get; set; }

    public string? District { get; set; }

    public string? Municipality { get; set; }

    public string? Parish { get; set; }

    public bool IsAbroad { get; set; }

    public int AlbumCount { get => Albums.Count; }

    public HashSet<Album> Albums { get; init; } = new();

    public int TracksCount { get => Tracks.Count; }

    public HashSet<Track> Tracks { get; init; } = new();

    private string GetDebuggerDisplay() => Name;

    public string GetLetterAvatar()
    {
        Span<char> chars = stackalloc char[3];
        int i = 0;
        foreach (var part in ShortName.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            if (ShouldInclude(part))
            {
                chars[i++] = part[0];
                if (i >= chars.Length)
                    break;
            }
        }

        return new string(chars[..i]);

        static bool ShouldInclude(string str)
        {
            return str.Length >= 3
                && str.All(Char.IsLetter)
                && !str.Equals("dos", StringComparison.InvariantCultureIgnoreCase)
                && !str.Equals("das", StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public string GetLocation()
    {
        return String.Join(", ", GetLocationParts(this));

        static IEnumerable<string> GetLocationParts(Artist artist)
        {
            if (!String.IsNullOrWhiteSpace(artist.District))
                yield return artist.District;
            if (!String.IsNullOrWhiteSpace(artist.Municipality))
                yield return artist.Municipality;
            if (!String.IsNullOrWhiteSpace(artist.Parish))
                yield return artist.Parish;

        }
    }
}