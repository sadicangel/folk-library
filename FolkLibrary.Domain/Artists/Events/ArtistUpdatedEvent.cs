namespace FolkLibrary.Artists.Events;

public sealed class ArtistUpdatedEvent : DomainEvent
{
    public override string Type { get; init; } = "artist.updated";

    public required string ArtistId { get; init; }
}
