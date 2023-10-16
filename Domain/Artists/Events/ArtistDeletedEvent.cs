namespace FolkLibrary.Artists.Events;

public sealed class ArtistDeletedEvent : DomainEvent
{
    public override string Type { get; init; } = "artist.deleted";

    public required string ArtistId { get; init; }
}