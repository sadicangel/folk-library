using FastEndpoints;
using FolkLibrary.Messaging;

namespace FolkLibrary.Artists.Events;

public sealed class ArtistDeletedNotificationHandler : IEventHandler<ArtistDeletedEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ArtistDeletedNotificationHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task HandleAsync(ArtistDeletedEvent notification, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var publisher = scope.Resolve<IArtistDeletedEventPublisher>();
        await publisher.PublishAsync(notification, cancellationToken);
    }
}
