namespace FolkLibrary.Tracks;

public sealed record class TrackInfoUpdated(
    string? Name,
    int? Number,
    string? Description,
    int? Year,
    TimeSpan? Duration)
{
    public Track Apply(Track aggregate) => aggregate with
    {
        Name = Name ?? aggregate.Name,
        Number = Number ?? aggregate.Number,
        Description = Description ?? aggregate.Description,
        Year = Year ?? aggregate.Year,
        IsYearUncertain = (Year ?? aggregate.Year) is null,
        Duration = Duration ?? aggregate.Duration
    };
}