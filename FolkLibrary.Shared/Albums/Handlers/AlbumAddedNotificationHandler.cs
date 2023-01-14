using FolkLibrary.Albums.Events;
using FolkLibrary.Interfaces;
using MediatR;

namespace FolkLibrary.Albums.Handlers;
public sealed class AlbumAddedNotificationHandler : INotificationHandler<AlbumAddedEvent>
{
    private readonly IEventPublisher _eventPublisher;

    public AlbumAddedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(AlbumAddedEvent notification, CancellationToken cancellationToken)
    {
        await _eventPublisher.PublishAsync(notification, cancellationToken);
    }
}
