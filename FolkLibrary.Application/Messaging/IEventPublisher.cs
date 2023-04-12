namespace FolkLibrary.Messaging;

public interface IEventPublisher<TEvent> where TEvent : DomainEvent
{
    Task PublishAsync(TEvent @event, CancellationToken cancellationToken = default);
}