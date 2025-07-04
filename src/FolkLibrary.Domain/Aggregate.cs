namespace FolkLibrary.Domain;

public abstract class Aggregate<TAggregate>
{
    private readonly Queue<DomainEvent<TAggregate>> _pendingEvents = [];

    public Guid Id { get; set; }

    public IEnumerable<DomainEvent<TAggregate>> EnumerateEvents => _pendingEvents.AsEnumerable();

    public DomainEvent<TAggregate>[] DequeueAllEvents()
    {
        var events = _pendingEvents.ToArray();
        _pendingEvents.Clear();
        return events;
    }

    protected TAggregate EnqueueAndApply(DomainEvent<TAggregate> @event)
    {
        _pendingEvents.Enqueue(@event);
        return Apply(@event);
    }

    protected abstract TAggregate Apply(DomainEvent<TAggregate> @event);
}
public abstract record class DomainEvent<TAggregate>
{
    public abstract TAggregate Apply(TAggregate aggregate);
}

public interface ICreatedEvent;