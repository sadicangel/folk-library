using FolkLibrary.Interfaces;
using MediatR;
using StronglyTypedIds;

namespace FolkLibrary;

[StronglyTypedId(StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.SystemTextJson | StronglyTypedIdConverter.EfCoreValueConverter)]
public readonly partial struct EventId : IId<EventId>
{
    public static EventId New(Guid guid) => new(guid);
}

public abstract class DomainEvent : INotification
{
    public EventId Id { get; init; } = EventId.New();
    public string Type { get; init; } = null!;
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

public abstract class DomainEvent<T> : DomainEvent
{
    public T Data { get; init; } = default!;
}
