using FolkLibrary.Interfaces;

namespace FolkLibrary.Artists.Events;

public sealed class ArtistCreatedEvent : DomainEvent<ArtistCreatedEventData>
{
    public ArtistCreatedEvent() => Type = "artist.created";
}

public sealed class ArtistCreatedEventData : IMapFrom<Artist>, IMapTo<ArtistDocument>
{
    public ArtistId Id { get; init; }

    public string Name { get; init; } = null!;

    public string ShortName { get; init; } = null!;

    public string? Description { get; init; }

    public int? Year { get; init; }

    public bool IsYearUncertain { get; init; }

    public HashSet<Genre> Genres { get; init; } = null!;

    public string Country { get; init; } = null!;

    public string? District { get; init; }

    public string? Municipality { get; init; }

    public string? Parish { get; init; }

    public bool IsAbroad { get; init; }
}
