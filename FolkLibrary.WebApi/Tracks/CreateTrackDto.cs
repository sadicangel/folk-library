using FolkLibrary.Albums;

namespace FolkLibrary.Tracks;

public sealed class CreateTrackDto
{
    public required string Name { get; init; }

    public string? Description { get; init; }

    public int? Year { get; init; }

    public required HashSet<string> Genres { get; init; }

    public required int Number { get; init; }

    public required TimeSpan Duration { get; init; }

    public Track ToTrack(Album album)
    {
        return new Track
        {
            Name = Name,
            Description = Description,
            Year = Year,
            IsYearUncertain = !Year.HasValue,
            Genres = Genres,
            Duration = Duration,
            Number = Number,
            Album = album
        };
    }
}