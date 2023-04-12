using FolkLibrary.Application.Interfaces;

namespace FolkLibrary.Tracks;

public sealed class CreateTrackDto : IMapTo<Track>
{
    public required string Name { get; init; }

    public string? Description { get; init; }

    public int? Year { get; init; }

    public bool IsYearUncertain { get; init; }

    public required HashSet<string> Genres { get; init; }

    public required int Number { get; init; }

    public required TimeSpan Duration { get; init; }
}