using Generator.Equals;

namespace FolkLibrary;

public sealed record class Artist(
    Guid Id,
    string Name,
    string ShortName,
    string LetterAvatar,
    string? Description,
    int? Year,
    bool IsYearUncertain,
    string YearString,
    bool IsAbroad,
    string Country,
    string? District,
    string? Municipality,
    string? Parish,
    string Location,
    [property: UnorderedEquality] List<string> Genres,
    [property: UnorderedEquality] List<Album> Albums
);