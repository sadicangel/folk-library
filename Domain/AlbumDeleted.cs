namespace FolkLibrary;

public sealed record class AlbumDeleted(Guid albumId)
{
    public Artist Apply(Artist artist)
    {
        var index = artist.Albums.FindIndex(a => a.Id == albumId);
        if (index >= 0)
            artist.Albums.RemoveAt(index);
        return artist;
    }
}
