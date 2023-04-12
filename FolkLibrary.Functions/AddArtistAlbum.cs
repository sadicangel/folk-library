using FolkLibrary.Artists.Commands;
using FolkLibrary.Artists.Events;
using MediatR;
using Microsoft.Azure.Functions.Worker;

namespace FolkLibrary.Functions;

public sealed class AddArtistAlbum
{
    private readonly ISender _mediator;

    public AddArtistAlbum(ISender mediator)
    {
        _mediator = mediator;
    }

    [Function(nameof(AddArtistAlbum))]
    public async Task Run([RabbitMQTrigger("artist.album.added.queue", ConnectionStringSetting = "RabbitMq")] ArtistAlbumAddedEvent @event)
    {
        await _mediator.Send(new IngestArtistAlbumAddedEventCommand { Event = @event });
    }
}
