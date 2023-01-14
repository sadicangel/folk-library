using FolkLibrary.Interfaces;
using FolkLibrary.Tracks;

namespace FolkLibrary.Albums;

public sealed class AlbumDocument : Document<AlbumId>, IMapFrom<Album>
{
    public int TrackCount { get; set; }

    public TimeSpan Duration { get; set; }

    public bool IsIncomplete { get; set; }

    public List<TrackDocument> Tracks { get; init; } = new();
}
