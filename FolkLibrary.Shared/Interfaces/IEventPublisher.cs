using FolkLibrary.Events;

namespace FolkLibrary.Interfaces;

public interface IEventPublisher
{
    void Publish<T>(DomainEvent<T> @event);
    Task PublishAsync<T>(DomainEvent<T> @event, CancellationToken cancellationToken = default);
}
