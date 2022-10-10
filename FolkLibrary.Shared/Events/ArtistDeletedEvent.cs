using FolkLibrary.Dtos;

namespace FolkLibrary.Events;

public sealed class ArtistDeletedEvent : DomainEvent<ArtistDto>
{
    public ArtistDeletedEvent() => Type = "artist.deleted";
}
