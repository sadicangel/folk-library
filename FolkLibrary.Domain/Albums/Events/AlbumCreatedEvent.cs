namespace FolkLibrary.Albums.Events;

public sealed class AlbumCreatedEvent : DomainEvent
{
    public override string Type { get; init; } = "album.created";

    public required string ArtistId { get; init; }

    public required string AlbumId { get; init; }
}