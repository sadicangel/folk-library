using FolkLibrary.Artists;

namespace FolkLibrary.Albums.Events;

public sealed class AlbumAddedEvent : DomainEvent<AlbumAddedEventData>
{
    public AlbumAddedEvent() => Type = "artist.album.added";
}

public sealed class AlbumAddedEventData
{
    public ArtistId ArtistId { get; init; }

    public AlbumId AlbumId { get; init; }
}
