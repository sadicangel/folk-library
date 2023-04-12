using EasyNetQ;
using FolkLibrary.Artists.Events;
using Microsoft.Extensions.Configuration;

namespace FolkLibrary.Messaging;

internal sealed class ArtistDeletedEventPublisher : RabbitMqEventPublisher<ArtistDeletedEvent>, IArtistDeletedEventPublisher
{
    public ArtistDeletedEventPublisher(IAdvancedBus bus, IConfiguration configuration) : base(bus, configuration.GetRequiredSection("RabbitMq:Topic").Value!)
    {
    }
}
