using FolkLibrary.Interfaces;

namespace FolkLibrary.Artists.Events;

public sealed class ArtistDeletedEvent : DomainEvent<ArtistDeletedEventData>
{
    public ArtistDeletedEvent() => Type = "artist.deleted";
}

public sealed class ArtistDeletedEventData : IMapTo<ArtistDocument>
{
    public ArtistId Id { get; }
}
