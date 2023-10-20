namespace FolkLibrary;
public sealed record class ArtistAddedToAlbum(Guid ArtistId)
{
    public Album Apply(Album aggregate)
    {
        aggregate.Artists.Add(ArtistId);
        return aggregate;
    }
}
