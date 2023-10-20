namespace FolkLibrary;

public sealed record class ArtistRemovedFromAlbum(Guid ArtistId)
{
    public Album Apply(Album aggregate)
    {
        aggregate.Artists.Remove(ArtistId);
        return aggregate;
    }
}
