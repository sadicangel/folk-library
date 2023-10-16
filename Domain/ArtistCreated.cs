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
        Description,
        Year,
        IsYearUncertain,
        IsAbroad,
        Country,
        District,
        Municipality,
        Parish,
        Genres,
        new List<Album>());
}