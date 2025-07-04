namespace FolkLibrary.Domain.Artists;

public sealed record class Location(
    string CountryCode,
    string CountryName,
    string District,
    string? Municipality,
    string? Parish
)
{
    public bool IsAbroad => CountryCode != "PT";
}
