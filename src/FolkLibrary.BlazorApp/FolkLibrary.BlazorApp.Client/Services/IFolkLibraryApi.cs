using Refit;

namespace FolkLibrary.BlazorApp.Client.Services;

public interface IFolkLibraryApi
{
    [Get("/api")]
    public Task<string> Hello();


    [Get("/api/artists")]
    public Task<List<ArtistView>> GetArtists();
}

public record ArtistView(
    Guid Id,
    string Name,
    string ShortName,
    string LetterAvatar,
    string? Description,
    int? Year,
    bool IsYearUncertain,
    string YearString,
    Location Location,
    List<AlbumView> Albums);

public record class Location(
    string CountryCode,
    string CountryName,
    string District,
    string? Municipality,
    string? Parish)
{
    public override string ToString()
    {
        return $"{CountryName}, {District}";
        //var builder = new StringBuilder($"{CountryName}-{District}");
        //if (!string.IsNullOrWhiteSpace(Municipality))
        //    builder.Append($"-{Municipality}");
        //if (!string.IsNullOrWhiteSpace(Parish))
        //    builder.Append($"-{Parish}");
        //return builder.ToString();
    }
}

public record AlbumView(
    Guid Id,
    string Name,
    string? Description,
    int? Year,
    bool IsYearUncertain,
    bool IsIncomplete,
    TimeSpan Duration,
    List<Track> Tracks,
    bool IsCompilation);

public record class Track(
    string Name,
    int Number,
    string? Description,
    int? Year,
    bool IsYearUncertain,
    TimeSpan Duration);

