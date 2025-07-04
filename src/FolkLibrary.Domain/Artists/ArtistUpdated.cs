namespace FolkLibrary.Domain.Artists;

public sealed record class ArtistUpdated(
    string? Name,
    string? ShortName,
    string? Description,
    int? Year,
    bool? IsYearUncertain,
    Location? Location) : DomainEvent<Artist>
{
    public override Artist Apply(Artist aggregate)
    {
        aggregate.Name = Name ?? aggregate.Name;
        aggregate.ShortName = ShortName ?? aggregate.ShortName;
        aggregate.LetterAvatar = GetLetterAvatar(ShortName ?? aggregate.ShortName);
        aggregate.Description = Description;
        aggregate.Year = Year;
        aggregate.IsYearUncertain = IsYearUncertain ?? aggregate.IsYearUncertain;
        aggregate.YearString = GetYearString(Year ?? aggregate.Year, IsYearUncertain ?? aggregate.IsYearUncertain);
        aggregate.Location = Location ?? aggregate.Location;

        return aggregate;
    }

    public string GetLetterAvatar(string shortName)
    {
        Span<char> chars = stackalloc char[3];
        var i = 0;
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

    public string GetYearString(int? year, bool isYearUncertain)
    {
        if (!Year.HasValue)
            return "";

        if (isYearUncertain)
            return FormattableString.Invariant($"{year}?");

        return FormattableString.Invariant($"{year}");
    }
}
