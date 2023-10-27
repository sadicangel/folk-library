using FolkLibrary.Artists;
using Marten.Events.Aggregation;

namespace FolkLibrary;

public sealed class ArtistProjection : SingleStreamProjection<Artist>
{
    public Artist Create(ArtistCreated @event) => @event.Create();

    public Artist Apply(ArtistInfoUpdated @event, Artist aggregate) => @event.Apply(aggregate);

    public Artist Apply(ArtistLocationUpdated @event, Artist aggregate) => @event.Apply(aggregate);

    public Artist Apply(ArtistAlbumUpdated @event, Artist aggregate) => @event.Apply(aggregate);

    public Artist Apply(ArtistAlbumAdded @event, Artist aggregate) => @event.Apply(aggregate);

    public Artist Apply(ArtistAlbumRemoved @event, Artist aggregate) => @event.Apply(aggregate);
}