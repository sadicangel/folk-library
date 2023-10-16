namespace FolkLibrary;

public sealed record class ArtistCreated(
    Guid Id,
    string Name,
    string ShortName,
    string? Description,
    int? Year,
    bool IsYearUncertain,
    bool IsAbroad,
    string Country,
    string? District,
    string? Municipality,
    string? Parish,
    List<string> Genres)
{
    public Artist Apply() => new(
        Id,
        Name,
        ShortName,
        ArtistUtil.GetLetterAvatar(ShortName),
        Description,
        Year,
        IsYearUncertain,
        ArtistUtil.GetYearString(Year, IsYearUncertain),
        IsAbroad,
        Country,
        District,
        Municipality,
        Parish,
        ArtistUtil.GetLocation(District, Municipality, Parish),
        Genres,
        new List<Album>());
}

file static class ArtistUtil
{
    public static string GetYearString(int? year, bool isYearUncertain)
    {
        if (!year.HasValue)
            return "";

        if (isYearUncertain)
            return FormattableString.Invariant($"{year}?");

        return FormattableString.Invariant($"{year}");
    }

    public static string GetLetterAvatar(string shortName)
    {
        Span<char> chars = stackalloc char[3];
        int i = 0;
        foreach (var part in shortName.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
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

    public static string GetLocation(string? district, string? municipality, string? parish)
    {
        return string.Join(", ", GetLocationParts(district, municipality, parish));

        static IEnumerable<string> GetLocationParts(string? district, string? municipality, string? parish)
        {
            if (!string.IsNullOrWhiteSpace(district))
                yield return district;
            if (!string.IsNullOrWhiteSpace(municipality))
                yield return municipality;
            if (!string.IsNullOrWhiteSpace(parish))
                yield return parish;

        }
    }
}