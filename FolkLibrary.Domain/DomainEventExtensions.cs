using EasyNetQ;
using System.Net.Mime;

namespace FolkLibrary;

public static class DomainEventExtensions
{
    public static Message<T> ToMessage<T>(this T domainEvent) where T : DomainEvent
    {
        return new Message<T>(domainEvent)
        {
            Properties =
            {
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTimeOffset.UtcNow.UtcTicks,
                ContentType = MediaTypeNames.Application.Json
            }
        };
    }
}