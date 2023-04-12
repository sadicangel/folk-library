using EasyNetQ;
using FolkLibrary.Artists.Events;
using Microsoft.Extensions.Configuration;

namespace FolkLibrary.Messaging;

internal sealed class ArtistUpdatedEventPublisher : RabbitMqEventPublisher<ArtistUpdatedEvent>, IArtistUpdatedEventPublisher
{
    public ArtistUpdatedEventPublisher(IAdvancedBus bus, IConfiguration configuration) : base(bus, configuration.GetRequiredSection("RabbitMq:Topic").Value!)
    {
    }
}
