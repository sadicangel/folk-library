namespace FolkLibrary;

public sealed record class AlbumDeleted(Guid AlbumId)
{
    public Artist Apply(Artist artist)
    {
        var index = artist.Albums.FindIndex(a => a.AlbumId == AlbumId);
        if (index >= 0)
            artist.Albums.RemoveAt(index);
        return artist;
    }
}
