namespace FolkLibrary.Artists.CreateArtist;

public sealed class CreateArtistRequest
{
    public required string Name { get; init; }
    public required string ShortName { get; init; }
    public string? Description { get; init; }
    public required int Year { get; init; }
    public bool IsYearUncertain { get; init; }
    public required HashSet<string> Genres { get; init; }
    public required string Country { get; init; }
    public string? District { get; init; }
    public string? Municipality { get; init; }
    public string? Parish { get; init; }
    public bool IsAbroad { get; init; }
}