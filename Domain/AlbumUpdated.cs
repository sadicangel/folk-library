namespace FolkLibrary;

public sealed record class AlbumUpdated(Album Album)
{
    public Artist Apply(Artist artist)
    {
        var index = artist.Albums.FindIndex(a => a.Id == Album.Id);
        if (index >= 0)
            artist.Albums[index] = Album;
        return artist;
    }
}