using FolkLibrary.Artists.Events;
using FolkLibrary.Interfaces;
using MediatR;

namespace FolkLibrary.Artists.Handlers;
public sealed class ArtistCreatedNotificationHandler : INotificationHandler<ArtistCreatedEvent>
{
    private readonly IEventPublisher _eventPublisher;

    public ArtistCreatedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(ArtistCreatedEvent notification, CancellationToken cancellationToken)
    {
        await _eventPublisher.PublishAsync(notification, cancellationToken);
    }
}
