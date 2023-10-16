namespace FolkLibrary;

public sealed record class AlbumCreated(Album Album)
{
    public Artist Apply(Artist artist)
    {
        artist.Albums.Add(Album);
        return artist;
    }
}
