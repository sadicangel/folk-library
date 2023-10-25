namespace FolkLibrary.Tracks;

public sealed record class TrackCreated(
    Guid TrackId,
    string Name,
    int Number,
    string? Description,
    int? Year,
    TimeSpan Duration)
{
    public Track Create() => new(
        TrackId,
        AlbumId: default,
        Name,
        Number,
        Description,
        Year,
        IsYearUncertain: Year is null,
        Duration
    );
}