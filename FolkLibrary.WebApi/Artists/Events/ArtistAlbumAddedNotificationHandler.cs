using FastEndpoints;
using FolkLibrary.Messaging;

namespace FolkLibrary.Artists.Events;
public sealed class ArtistAlbumAddedNotificationHandler : IEventHandler<ArtistAlbumAddedEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ArtistAlbumAddedNotificationHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task HandleAsync(ArtistAlbumAddedEvent @event, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var publisher = scope.Resolve<IArtistAlbumAddedEventPublisher>();
        await publisher.PublishAsync(@event, cancellationToken);
    }
}
