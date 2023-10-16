namespace FolkLibrary;

public sealed record class AlbumCreated(
    Guid Id,
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
            Id: Id,
            Name: Name,
            Description: Description,
            Year: Year,
            IsYearUncertain: Year is null,
            IsCompilation: IsCompilation,
            IsIncomplete: false,
            Duration: TimeSpan.Zero,
            Genres: Genres,
            Tracks: Tracks
        ));
        return artist;
    }
}
