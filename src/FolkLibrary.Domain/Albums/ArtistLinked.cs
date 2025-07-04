namespace FolkLibrary.Domain.Albums;
public sealed record class ArtistLinked(Guid ArtistId) : DomainEvent<Album>
{
    public override Album Apply(Album aggregate)
    {
        aggregate.Artists.Add(ArtistId);
        return aggregate;
    }
}
