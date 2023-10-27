namespace FolkLibrary.Artists;

public sealed record class ArtistAlbumUpdated(Album Album)
{
    public Artist Apply(Artist aggregate)
    {
        var index = aggregate.Albums.FindIndex(a => a.AlbumId == Album.AlbumId);
        if (index >= 0)
            aggregate.Albums[index] = Album;
        return aggregate;
    }
}