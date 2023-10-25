namespace FolkLibrary.Tracks;
public sealed record class TrackAlbumUpdated(Guid AlbumId)
{
    public Track Apply(Track aggregate) => aggregate with { AlbumId = AlbumId };
}
