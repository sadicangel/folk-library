using FolkLibrary.Albums;
using Marten.Events.Aggregation;

namespace FolkLibrary;

public sealed class AlbumProjection : SingleStreamProjection<Album>
{
    public Album Create(AlbumCreated @event) => @event.Create();

    public Album Apply(AlbumInfoUpdated @event, Album aggregate) => @event.Apply(aggregate);

    public Album Apply(AlbumArtistAdded @event, Album aggregate) => @event.Apply(aggregate);

    public Album Apply(AlbumArtistRemoved @event, Album aggregate) => @event.Apply(aggregate);

    public Album Apply(AlbumTrackAdded @event, Album aggregate) => @event.Apply(aggregate);

    public Album Apply(AlbumTrackUpdated @event, Album aggregate) => @event.Apply(aggregate);
}