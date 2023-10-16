namespace FolkLibrary;

internal static class ModelExtensions
{
    private static string GetYearString(int? year, bool isYearUncertain)
    {
        if (!year.HasValue)
            return "";

        if (isYearUncertain)
            return FormattableString.Invariant($"{year}?");

        return FormattableString.Invariant($"{year}");
    }

    public static string GetYearString(this Artist artist) => GetYearString(artist.Year, artist.IsYearUncertain);
    public static string GetYearString(this Album artist) => GetYearString(artist.Year, artist.IsYearUncertain);
    public static string GetYearString(this Track artist) => GetYearString(artist.Year, artist.IsYearUncertain);

    public static string GetLetterAvatar(this Artist artist)
    {
        Span<char> chars = stackalloc char[3];
        int i = 0;
        foreach (var part in artist.ShortName.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
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
                && str.All(char.IsLetter)
                && !str.Equals("dos", StringComparison.InvariantCultureIgnoreCase)
                && !str.Equals("das", StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public static string GetLocation(this Artist artist)
    {
        return string.Join(", ", GetLocationParts(artist));

        static IEnumerable<string> GetLocationParts(Artist artist)
        {
            if (!string.IsNullOrWhiteSpace(artist.District))
                yield return artist.District;
            if (!string.IsNullOrWhiteSpace(artist.Municipality))
                yield return artist.Municipality;
            if (!string.IsNullOrWhiteSpace(artist.Parish))
                yield return artist.Parish;

        }
    }
}
