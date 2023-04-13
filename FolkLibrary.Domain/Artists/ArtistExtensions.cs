namespace FolkLibrary.Artists;

public static class ArtistExtensions
{
    public static string GetLetterAvatar(this Artist artist)
    {
        Span<char> chars = stackalloc char[3];
        int i = 0;
        foreach (var (part, _) in artist.ShortName.SplitLines())
        {
            if (part.Length > 3)
            {
                chars[i++] = part[0];
                if (i >= chars.Length)
                    break;
            }
        }
        return new string(chars[..i]);
    }

    public static string GetLocation(this Artist artist)
    {
        return String.Join(", ", GetLocationParts(artist));

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
