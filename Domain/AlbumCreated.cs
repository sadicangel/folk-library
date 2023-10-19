namespace FolkLibrary;

public sealed record class AlbumCreated(
    Guid AlbumId,
    string Name,
    string? Description,
    int? Year,
    bool IsCompilation,
    List<string> Genres,
    List<Track> Tracks)
{
    public Artist Apply(Artist artist)
    {
        artist.Albums.Add(new Album(
            AlbumId: AlbumId,
            Name: Name,
            Description: Description,
            Year: Year,
            IsYearUncertain: Year is null,
            IsCompilation: IsCompilation,
            IsIncomplete: Tracks.Count == 0 || Tracks.Max(t => t.Number) != Tracks.Count,
            Duration: Tracks.Count == 0 ? TimeSpan.Zero : Tracks.Aggregate(TimeSpan.Zero, (p, c) => p + c.Duration),
            Genres: Genres,
            Tracks: Tracks
        ));
        return artist;
    }
}
