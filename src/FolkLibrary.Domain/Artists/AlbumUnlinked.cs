namespace FolkLibrary.Domain.Artists;

public sealed record class AlbumUnlinked(Guid AlbumId) : DomainEvent<Artist>
{
    public override Artist Apply(Artist aggregate)
    {
        aggregate.Albums = aggregate.Albums.Remove(AlbumId);
        return aggregate;
    }
}
