using FolkLibrary.Dtos;

namespace FolkLibrary.Events;

public sealed class ArtistCreatedEvent : DomainEvent<ArtistDto>
{
    public ArtistCreatedEvent() => Type = "artist.created";
}
