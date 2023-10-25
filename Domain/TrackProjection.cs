using FolkLibrary.Tracks;
using Marten.Events.Aggregation;

namespace FolkLibrary;

public sealed class TrackProjection : SingleStreamProjection<Track>
{
    public Track Create(TrackCreated @event) => @event.Create();

    public Track Apply(TrackInfoUpdated @event, Track aggregate) => @event.Apply(aggregate);

    public Track Apply(TrackAlbumUpdated @event, Track aggregate) => @event.Apply(aggregate);
}