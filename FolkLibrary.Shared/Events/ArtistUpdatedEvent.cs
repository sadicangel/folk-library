using FolkLibrary.Dtos;

namespace FolkLibrary.Events;

public sealed class ArtistUpdatedEvent : DomainEvent<ArtistDto>
{
    public ArtistUpdatedEvent() => Type = "artist.updated";
}
