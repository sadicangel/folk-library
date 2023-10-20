using Marten.Events.Aggregation;

namespace FolkLibrary;

public sealed class ArtistProjection : SingleStreamProjection<Artist>
{
    public Artist Create(ArtistCreated @event) => @event.Create();

    public Artist Apply(ArtistInfoUpdated @event, Artist aggregate) => @event.Apply(aggregate);

    public Artist Apply(ArtistLocationUpdated @event, Artist aggregate) => @event.Apply(aggregate);

    public Artist Apply(AlbumUpdated @event, Artist aggregate) => @event.Apply(aggregate);

    public Artist Apply(AlbumAddedToArtist @event, Artist aggregate) => @event.Apply(aggregate);

    public Artist Apply(AlbumRemovedFromArtist @event, Artist aggregate) => @event.Apply(aggregate);
}