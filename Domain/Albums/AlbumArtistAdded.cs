namespace FolkLibrary.Albums;
public sealed record class AlbumArtistAdded(Guid ArtistId)
{
    public Album Apply(Album aggregate)
    {
        if (!aggregate.Artists.Contains(ArtistId))
            aggregate.Artists.Add(ArtistId);
        return aggregate;
    }
}
