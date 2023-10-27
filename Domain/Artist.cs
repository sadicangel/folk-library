using Generator.Equals;

namespace FolkLibrary;

public sealed record class Artist(
    Guid ArtistId,
    string Name,
    string ShortName,
    string LetterAvatar,
    string? Description,
    int? Year,
    bool IsYearUncertain,
    string YearString,
    Location Location,
    [property: UnorderedEquality] List<Album> Albums
)
{
    public bool IsAbroad { get => Location is not { CountryCode: "PT" }; }
}