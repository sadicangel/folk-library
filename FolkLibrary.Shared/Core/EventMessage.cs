using EasyNetQ;
using System.Net.Mime;

namespace FolkLibrary;

public sealed class EventMessage<T> : IMessage<T> where T : DomainEvent
{
    public T Body { get; init; } = default!;
    public Type MessageType { get => typeof(T); }
    public MessageProperties Properties { get; } = null!;

    public EventMessage() { }

    public EventMessage(T @event)
    {
        Properties = new MessageProperties
        {
            MessageId = @event.ToString(),
            Type = @event.Type,
            Timestamp = @event.Timestamp.UtcTicks,
            ContentType = MediaTypeNames.Application.Json,
        };
        Body = @event;
    }

    public object GetBody() => Body!;
}
