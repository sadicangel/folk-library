namespace FolkLibrary;

public sealed record class ArtistUpdated(
    string? Name,
    string? ShortName,
    string? Description,
    int? Year,
    bool? IsYearUncertain,
    bool? IsAbroad,
    string? Country,
    string? District,
    string? Municipality,
    string? Parish)
{
    public Artist Apply(Artist artist) => artist with
    {
        Name = Name ?? artist.Name,
        ShortName = ShortName ?? artist.ShortName,
        Description = Description ?? artist.Description,
        Year = Year ?? artist.Year,
        IsYearUncertain = IsYearUncertain ?? artist.IsYearUncertain,
        IsAbroad = IsAbroad ?? artist.IsAbroad,
        Country = Country ?? artist.Country,
        District = District ?? artist.District,
        Municipality = Municipality ?? artist.Municipality,
        Parish = Parish ?? artist.Parish
    };
}
