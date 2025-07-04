using FolkLibrary.Domain;

namespace FolkLibrary.Domain.Artists;
public sealed record class AlbumLinked(Guid AlbumId) : DomainEvent<Artist>
{
    public override Artist Apply(Artist aggregate)
    {
        aggregate.Albums = aggregate.Albums.Add(AlbumId);
        return aggregate;
    }
}
