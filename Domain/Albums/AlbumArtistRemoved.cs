namespace FolkLibrary.Albums;

public sealed record class AlbumArtistRemoved(Guid ArtistId)
{
    public Album Apply(Album aggregate)
    {
        aggregate.Artists.Remove(ArtistId);
        return aggregate;
    }
}
