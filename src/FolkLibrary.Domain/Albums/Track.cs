namespace FolkLibrary.Domain.Albums;

public sealed record class Track(
    string Name,
    int Number,
    string? Description,
    int? Year,
    bool IsYearUncertain,
    TimeSpan Duration
);