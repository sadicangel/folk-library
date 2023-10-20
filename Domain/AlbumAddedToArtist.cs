namespace FolkLibrary;
public sealed record class AlbumAddedToArtist(Album Album)
{
    public Artist Apply(Artist aggregate)
    {
        aggregate.Albums.Add(Album);
        return aggregate;
    }
}
