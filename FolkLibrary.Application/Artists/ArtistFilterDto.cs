namespace FolkLibrary.Artists;

public sealed class ArtistFilterDto
{
    public string? Country { get; init; }
    public string? District { get; init; }
    public string? Municipality { get; init; }
    public string? Parish { get; init; }
    public int? AfterYear { get; init; }
    public int? BeforeYear { get; init; }
}
