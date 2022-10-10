using EasyNetQ;
using System.Net.Mime;

namespace FolkLibrary.Events;

public abstract class DomainEvent<T> : IMessage<T>
{
    public string Id { get => Properties.MessageId; init => Properties.MessageId = value; }
    public string Type { get => Properties.Type; init => Properties.Type = value; }
    public DateTimeOffset Created { get => new(Properties.Timestamp, TimeSpan.Zero); init => Properties.Timestamp = value.UtcTicks; }
    public T Body { get; init; } = default!;
    public Type MessageType { get => typeof(T); }
    public MessageProperties Properties { get; } = new MessageProperties
    {
        ContentType = MediaTypeNames.Application.Json,
        MessageId = Guid.NewGuid().ToString(),
        Timestamp = DateTimeOffset.UtcNow.UtcTicks,
    };

    public object GetBody() => Body!;
}
