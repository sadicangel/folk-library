namespace FolkLibrary.Albums.Events;

public sealed class CompilationAlbumCreatedEvent : DomainEvent
{
    public override string Type { get; init; } = "album.compilation.created";

    public required Dictionary<string, List<int>> TracksByArtistId { get; init; }

    public required string AlbumId { get; init; }
}