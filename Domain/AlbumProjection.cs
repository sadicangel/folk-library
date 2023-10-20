using Marten.Events.Aggregation;

namespace FolkLibrary;

public sealed class AlbumProjection : SingleStreamProjection<Album>
{
    public Album Create(AlbumCreated @event) => @event.Create();

    public Album Apply(AlbumUpdated @event, Album aggregate) => @event.Apply(aggregate);

    public Album Apply(ArtistAddedToAlbum @event, Album aggregate) => @event.Apply(aggregate);

    public Album Apply(ArtistRemovedFromAlbum @event, Album aggregate) => @event.Apply(aggregate);
}