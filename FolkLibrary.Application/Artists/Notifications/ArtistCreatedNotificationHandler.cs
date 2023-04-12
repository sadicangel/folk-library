using FolkLibrary.Artists.Events;
using FolkLibrary.Messaging;
using MediatR;

namespace FolkLibrary.Artists.Handlers;
public sealed class ArtistCreatedNotificationHandler : INotificationHandler<ArtistCreatedEvent>
{
    private readonly IArtistCreatedEventPublisher _artistCreatedEventPublisher;

    public ArtistCreatedNotificationHandler(IArtistCreatedEventPublisher artistCreatedEventPublisher)
    {
        _artistCreatedEventPublisher = artistCreatedEventPublisher;
    }

    public async Task Handle(ArtistCreatedEvent notification, CancellationToken cancellationToken)
    {
        await _artistCreatedEventPublisher.PublishAsync(notification, cancellationToken);
    }
}
