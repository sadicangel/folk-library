namespace FolkLibrary;

public sealed record class AlbumCreated(
    Guid AlbumId,
    string Name,
    string? Description,
    int? Year,
    List<Track> Tracks)
{
    public Album Create()
    {
        return new Album(
            AlbumId: AlbumId,
            Name: Name,
            Description: Description,
            Year: Year,
            IsYearUncertain: Year is null,
            IsCompilation: false,
            IsIncomplete: Tracks.Count == 0 || Tracks.Max(t => t.Number) != Tracks.Count,
            Duration: Tracks.Count == 0 ? TimeSpan.Zero : Tracks.Aggregate(TimeSpan.Zero, (p, c) => p + c.Duration),
            Artists: new List<Guid>(),
            Tracks: Tracks
        );
    }
}
