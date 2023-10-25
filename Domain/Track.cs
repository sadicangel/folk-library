namespace FolkLibrary;

public sealed record class Track(
    Guid TrackId,
    Guid AlbumId,
    string Name,
    int Number,
    string? Description,
    int? Year,
    bool IsYearUncertain,
    TimeSpan Duration
);