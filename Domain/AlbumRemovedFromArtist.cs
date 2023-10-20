namespace FolkLibrary;

public sealed record class AlbumRemovedFromArtist(Guid AlbumId)
{
    public Artist Apply(Artist aggregate)
    {
        var index = aggregate.Albums.FindIndex(a => a.AlbumId == AlbumId);
        if (index >= 0)
            aggregate.Albums.RemoveAt(index);
        return aggregate;
    }
}