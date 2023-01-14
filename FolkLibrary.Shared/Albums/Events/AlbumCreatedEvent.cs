using FolkLibrary.Interfaces;
using FolkLibrary.Tracks;

namespace FolkLibrary.Albums.Events;

public sealed class AlbumCreatedEvent : DomainEvent<AlbumCreatedEventData>
{
    public AlbumCreatedEvent() => Type = "artist.updated";
}

public sealed class AlbumCreatedEventData : IMapFrom<Album>, IMapTo<AlbumDocument>
{
    public AlbumId Id { get; init; }

    public string Name { get; init; } = null!;

    public string? Description { get; init; }

    public int? Year { get; init; }

    public bool IsYearUncertain { get; init; }

    public HashSet<Genre> Genres { get; init; } = null!;

    public int TrackCount { get; init; }

    public TimeSpan Duration { get; init; }

    public bool IsIncomplete { get; init; }

    public List<TrackDocument> Tracks { get; init; } = null!;
}
