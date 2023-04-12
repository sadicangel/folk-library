using FolkLibrary.Artists.Events;
using FolkLibrary.Messaging;
using MediatR;

namespace FolkLibrary.Artists.Handlers;

public sealed class ArtistDeletedNotificationHandler : INotificationHandler<ArtistDeletedEvent>
{
    private readonly IArtistDeletedEventPublisher _artistDeletedEventPublisher;

    public ArtistDeletedNotificationHandler(IArtistDeletedEventPublisher artistDeletedEventPublisher)
    {
        _artistDeletedEventPublisher = artistDeletedEventPublisher;
    }

    public async Task Handle(ArtistDeletedEvent notification, CancellationToken cancellationToken)
    {
        await _artistDeletedEventPublisher.PublishAsync(notification, cancellationToken);
    }
}
