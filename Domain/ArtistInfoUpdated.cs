namespace FolkLibrary;

public sealed record class ArtistInfoUpdated(
    string? Name,
    string? ShortName,
    string? Description,
    int? Year,
    bool? IsYearUncertain)
{
    public Artist Apply(Artist artist) => artist with
    {
        Name = Name ?? artist.Name,
        ShortName = ShortName ?? artist.ShortName,
        Description = Description ?? artist.Description,
        Year = Year ?? artist.Year,
        IsYearUncertain = IsYearUncertain ?? artist.IsYearUncertain
    };
}