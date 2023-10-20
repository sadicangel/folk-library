namespace FolkLibrary;

public sealed record class ArtistLocationUpdated(string? CountryCode, string? CountryName, string? District, string? Municipality, string? Parish)
{
    public Artist Apply(Artist artist) => artist with
    {
        Location = artist.Location with
        {
            CountryCode = CountryCode ?? artist.Location.CountryCode,
            CountryName = CountryName ?? artist.Location.CountryName,
            District = District ?? artist.Location.District,
            Municipality = Municipality ?? artist.Location.Municipality,
            Parish = Parish ?? artist.Location.Parish,
        }
    };
}