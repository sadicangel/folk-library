namespace FolkLibrary.Tracks;

public sealed class CreateTrackDto
{
    public required string Name { get; init; }

    public string? Description { get; init; }

    public int? Year { get; init; }

    public required List<string> Genres { get; init; }

    public required int Number { get; init; }

    public required TimeSpan Duration { get; init; }
}