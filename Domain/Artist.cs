using Generator.Equals;

namespace FolkLibrary;

public sealed record class Artist(
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
    [property: UnorderedEquality] List<string> Genres,
    [property: UnorderedEquality] List<Album> Albums
);