namespace FolkLibrary.Domain.Albums;

public sealed record class ArtistUnlinked(Guid ArtistId) : DomainEvent<Album>
{
    public override Album Apply(Album aggregate)
    {
        aggregate.Artists.Remove(ArtistId);

        return aggregate;
    }
}
