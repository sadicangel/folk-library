namespace FolkLibrary.Artists.UpdateArtist;

public sealed class UpdateArtistRequest
{
    public /*required*/ string ArtistId { get; init; } = String.Empty;
    public string? Name { get; init; }
    public string? ShortName { get; init; }
    public string? Description { get; init; }
    public int? Year { get; init; }
    public bool? IsYearUncertain { get; init; }
    public HashSet<string>? Genres { get; init; }
    public string? Country { get; init; }
    public string? District { get; init; }
    public string? Municipality { get; init; }
    public string? Parish { get; init; }
    public bool? IsAbroad { get; init; }
}
