using Marten.Events.Aggregation;

namespace FolkLibrary.Domain.Artists;

public sealed class ArtistProjection : SingleStreamProjection<Artist, Guid>
{
    public Artist Apply(ArtistCreated @event, Artist aggregate) => @event.Apply(aggregate);

    public Artist Apply(ArtistUpdated @event, Artist aggregate) => @event.Apply(aggregate);

    public Artist Apply(AlbumLinked @event, Artist aggregate) => @event.Apply(aggregate);

    public Artist Apply(AlbumUnlinked @event, Artist aggregate) => @event.Apply(aggregate);
}
