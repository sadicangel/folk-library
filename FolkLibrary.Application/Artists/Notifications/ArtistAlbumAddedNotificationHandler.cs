using FolkLibrary.Artists.Events;
using FolkLibrary.Messaging;
using MediatR;

namespace FolkLibrary.Artists.Notifications;
public sealed class ArtistAlbumAddedNotificationHandler : INotificationHandler<ArtistAlbumAddedEvent>
{
    private readonly IArtistAlbumAddedEventPublisher _artistAlbumAddedEventPublisher;

    public ArtistAlbumAddedNotificationHandler(IArtistAlbumAddedEventPublisher artistAlbumAddedEventPublisher)
    {
        _artistAlbumAddedEventPublisher = artistAlbumAddedEventPublisher;
    }

    public async Task Handle(ArtistAlbumAddedEvent notification, CancellationToken cancellationToken)
    {
        await _artistAlbumAddedEventPublisher.PublishAsync(notification, cancellationToken);
    }
}
