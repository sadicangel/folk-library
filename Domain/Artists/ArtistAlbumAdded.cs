namespace FolkLibrary.Artists;
public sealed record class ArtistAlbumAdded(Album Album)
{
    public Artist Apply(Artist aggregate)
    {
        var index = aggregate.Albums.FindIndex(a => a.AlbumId == Album.AlbumId);
        if (index >= 0)
            aggregate.Albums[index] = Album;
        else
            aggregate.Albums.Add(Album);
        return aggregate;
    }
}
