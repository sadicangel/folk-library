namespace FolkLibrary.Albums;

public sealed record class AlbumTrackUpdated(Track Track)
{
    public Album Apply(Album aggregate)
    {
        var index = aggregate.Tracks.IndexOf(Track);
        if (index >= 0)
            aggregate.Tracks[index] = Track;
        return aggregate;
    }
}