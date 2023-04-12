using MediatR;
using System.Net.Mime;

namespace FolkLibrary;

public abstract class DomainEvent : INotification
{
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public abstract string Type { get; init; }

    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;

    public string ContentType { get; init; } = MediaTypeNames.Application.Json;
}