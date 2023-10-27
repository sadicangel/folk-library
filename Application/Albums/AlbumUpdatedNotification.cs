using FolkLibrary.Artists;
using Marten;
using MediatR;

namespace FolkLibrary.Albums;

public sealed record class AlbumUpdatedNotification(Guid AlbumId) : INotification;

public sealed class AlbumUpdatedNotificationHandler : INotificationHandler<AlbumUpdatedNotification>
{
    private readonly IDocumentSession _documentSession;

    public AlbumUpdatedNotificationHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task Handle(AlbumUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var album = await _documentSession.Events.AggregateStreamAsync<Album>(notification.AlbumId, token: cancellationToken)
            ?? throw new FolkLibraryException($"Album {notification.AlbumId} does not exist");

        var artisAlbumUpdated = new ArtistAlbumUpdated(album);
        foreach (var artistId in album.Artists)
            await _documentSession.Events.AppendOptimistic(artistId, artisAlbumUpdated);
    }
}