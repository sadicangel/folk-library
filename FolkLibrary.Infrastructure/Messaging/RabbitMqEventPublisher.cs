using EasyNetQ;
using EasyNetQ.Topology;

namespace FolkLibrary.Messaging;
internal abstract class RabbitMqEventPublisher<T> : IEventPublisher<T> where T : DomainEvent
{
    private readonly Exchange _exchange;
    private readonly IAdvancedBus _bus;

    public RabbitMqEventPublisher(IAdvancedBus bus, string topicName)
    {
        _bus = bus;
        _exchange = _bus.ExchangeDeclare(topicName, "topic");
    }

    public Task PublishAsync(T @event, CancellationToken cancellationToken = default) =>
        _bus.PublishAsync(_exchange, @event.Type, mandatory: false, @event.ToMessage(), cancellationToken);
}