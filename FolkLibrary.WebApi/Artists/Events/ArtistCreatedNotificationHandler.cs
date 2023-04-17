using FastEndpoints;
using FolkLibrary.Messaging;

namespace FolkLibrary.Artists.Events;
public sealed class ArtistCreatedNotificationHandler : IEventHandler<ArtistCreatedEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ArtistCreatedNotificationHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task HandleAsync(ArtistCreatedEvent notification, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var publisher = scope.Resolve<IArtistCreatedEventPublisher>();
        await publisher.PublishAsync(notification, cancellationToken);
    }
}
