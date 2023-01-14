using FolkLibrary.Interfaces;

namespace FolkLibrary.Tracks;

public sealed class TrackDocument : Document<TrackId>, IMapFrom<Track>
{
    public int Number { get; set; }

    public TimeSpan Duration { get; set; }
}