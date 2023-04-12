using FolkLibrary.Artists.Events;
using FolkLibrary.Messaging;
using MediatR;

namespace FolkLibrary.Artists.Handlers;

public sealed class ArtistUpdatedNotificationHandler : INotificationHandler<ArtistUpdatedEvent>
{
    private readonly IArtistUpdatedEventPublisher _artistUpdatedEventPublisher;

    public ArtistUpdatedNotificationHandler(IArtistUpdatedEventPublisher artistUpdatedEventPublisher)
    {
        _artistUpdatedEventPublisher = artistUpdatedEventPublisher;
    }

    public async Task Handle(ArtistUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await _artistUpdatedEventPublisher.PublishAsync(notification, cancellationToken);
    }
}
