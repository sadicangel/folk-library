namespace FolkLibrary.Interfaces;

public interface IEventPublisher
{
    void Publish<TEvent>(TEvent @event) where TEvent : DomainEvent;
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : DomainEvent;
}
