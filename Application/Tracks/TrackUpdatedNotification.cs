using FolkLibrary.Albums;
using Marten;
using MediatR;

namespace FolkLibrary.Tracks;
public sealed record class TrackUpdatedNotification(Guid TrackId) : INotification;

public sealed class TrackUpdatedNotificationHandler : INotificationHandler<TrackUpdatedNotification>
{
    private readonly IDocumentSession _documentSession;

    public TrackUpdatedNotificationHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task Handle(TrackUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var track = await _documentSession.Events.AggregateStreamAsync<Track>(notification.TrackId, token: cancellationToken)
            ?? throw new FolkLibraryException($"Track {notification.TrackId} does not exist");

        var albumTrackUpdated = new AlbumTrackUpdated(track);
        await _documentSession.Events.AppendOptimistic(track.AlbumId, albumTrackUpdated);
        await _documentSession.SaveChangesAsync(cancellationToken);
    }
}
