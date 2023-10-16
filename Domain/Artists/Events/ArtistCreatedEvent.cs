namespace FolkLibrary.Artists.Events;

public sealed class ArtistCreatedEvent : DomainEvent
{
    public override string Type { get; init; } = "artist.created";

    public required string ArtistId { get; init; }
}