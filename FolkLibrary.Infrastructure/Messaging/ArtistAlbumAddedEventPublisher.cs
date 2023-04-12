using EasyNetQ;
using FolkLibrary.Artists.Events;
using Microsoft.Extensions.Configuration;

namespace FolkLibrary.Messaging;

internal sealed class ArtistAlbumAddedEventPublisher : RabbitMqEventPublisher<ArtistAlbumAddedEvent>, IArtistAlbumAddedEventPublisher
{
    public ArtistAlbumAddedEventPublisher(IAdvancedBus bus, IConfiguration configuration) : base(bus, configuration.GetRequiredSection("RabbitMq:Topic").Value!)
    {
    }
}