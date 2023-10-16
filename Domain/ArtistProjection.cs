using Marten.Events.Aggregation;

namespace FolkLibrary;

public sealed class ArtistProjection : SingleStreamProjection<Artist>
{
    public Artist Create(ArtistCreated @event) => @event.Apply();

    public Artist Apply(ArtistUpdated @event, Artist aggregate) => @event.Apply(aggregate);


    public Artist Apply(AlbumCreated @event, Artist aggregate) => @event.Apply(aggregate);

    public Artist Apply(AlbumUpdated @event, Artist aggregate) => @event.Apply(aggregate);

    public Artist Apply(AlbumDeleted @event, Artist aggregate) => @event.Apply(aggregate);
}