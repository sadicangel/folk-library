namespace FolkLibrary.Albums;

public sealed record class AlbumCreated(
    Guid AlbumId,
    string Name,
    string? Description,
    int? Year)
{
    public Album Create()
    {
        return new Album(
            AlbumId: AlbumId,
            Name: Name,
            Description: Description,
            Year: Year,
            IsYearUncertain: Year is null,
            IsIncomplete: true,
            Duration: TimeSpan.Zero,
            Artists: new List<Guid>(),
            Tracks: new List<Track>()
        );
    }
}
