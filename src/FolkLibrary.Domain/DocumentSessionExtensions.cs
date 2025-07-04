using Marten;

namespace FolkLibrary.Domain;

public static class DocumentSessionExtensions
{
    public static async Task QueueChangesAsync<T>(this IDocumentSession documentSession, T aggregate, CancellationToken cancellationToken = default)
        where T : Aggregate<T>
    {
        var events = aggregate.DequeueAllEvents();
        if (events.Length == 0)
            return;

        if (events[0] is ICreatedEvent)
            documentSession.Events.StartStream<T>(aggregate.Id, events);
        else
        {
            await documentSession.Events.AppendOptimistic(aggregate.Id, cancellationToken, events);
        }
    }

    public static async Task SaveChangesAsync<T>(this IDocumentSession documentSession, T aggregate, CancellationToken cancellationToken = default)
        where T : Aggregate<T>
    {
        await documentSession.QueueChangesAsync(aggregate, cancellationToken);
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
