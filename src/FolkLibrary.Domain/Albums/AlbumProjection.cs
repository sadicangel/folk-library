using Marten.Events.Aggregation;

namespace FolkLibrary.Domain.Albums;

public sealed class AlbumProjection : SingleStreamProjection<Album, Guid>
{
    public Album Apply(AlbumCreated @event, Album aggregate) => @event.Apply(aggregate);

    public Album Apply(AlbumUpdated @event, Album aggregate) => @event.Apply(aggregate);

    public Album Apply(ArtistLinked @event, Album aggregate) => @event.Apply(aggregate);

    public Album Apply(ArtistUnlinked @event, Album aggregate) => @event.Apply(aggregate);

    public Album Apply(TrackAdded @event, Album aggregate) => @event.Apply(aggregate);
}
