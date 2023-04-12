using EasyNetQ;
using FolkLibrary.Artists.Events;
using Microsoft.Extensions.Configuration;

namespace FolkLibrary.Messaging;

internal sealed class ArtistCreatedEventPublisher : RabbitMqEventPublisher<ArtistCreatedEvent>, IArtistCreatedEventPublisher
{
    public ArtistCreatedEventPublisher(IAdvancedBus bus, IConfiguration configuration) : base(bus, configuration.GetRequiredSection("RabbitMq:Topic").Value!)
    {
    }
}