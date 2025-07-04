namespace FolkLibrary.Domain.Artists;

public sealed record class ArtistCreated(
    Guid Id,
    string Name,
    string ShortName,
    string? Description,
    int? Year,
    bool IsYearUncertain,
    Location Location) : DomainEvent<Artist>, ICreatedEvent
{
    public override Artist Apply(Artist aggregate)
    {
        aggregate.Id = Id;
        aggregate.Name = Name;
        aggregate.ShortName = ShortName;
        aggregate.LetterAvatar = GetLetterAvatar();
        aggregate.Description = Description;
        aggregate.Year = Year;
        aggregate.IsYearUncertain = IsYearUncertain;
        aggregate.YearString = GetYearString();
        aggregate.Location = Location;
        aggregate.Albums = [];

        return aggregate;
    }

    public string GetLetterAvatar()
    {
        Span<char> chars = stackalloc char[3];
        var i = 0;
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
                && str.All(char.IsLetter)
                && !str.Equals("dos", StringComparison.InvariantCultureIgnoreCase)
                && !str.Equals("das", StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public string GetYearString()
    {
        if (!Year.HasValue)
            return "";

        if (IsYearUncertain)
            return FormattableString.Invariant($"{Year}?");

        return FormattableString.Invariant($"{Year}");
    }
}
