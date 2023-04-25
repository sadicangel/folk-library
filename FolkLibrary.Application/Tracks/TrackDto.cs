namespace FolkLibrary.Tracks;

public sealed class TrackDto : IDocument
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public string? Description { get; set; }

    public int? Year { get; set; }

    public bool IsYearUncertain { get; set; }

    public required List<string> Genres { get; init; }

    public required int Number { get; set; }

    public required TimeSpan Duration { get; set; }
}