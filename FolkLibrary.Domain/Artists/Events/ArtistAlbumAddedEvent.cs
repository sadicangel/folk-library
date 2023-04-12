namespace FolkLibrary.Artists.Events;

public sealed class ArtistAlbumAddedEvent : DomainEvent
{
    public override string Type { get; init; } = "artist.album.added";

    public required string ArtistId { get; init; }

    public required string AlbumId { get; init; }
}