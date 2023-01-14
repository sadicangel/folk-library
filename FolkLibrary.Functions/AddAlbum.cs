using FolkLibrary.Albums.Commands;
using FolkLibrary.Albums.Events;
using MediatR;
using Microsoft.Azure.Functions.Worker;

namespace FolkLibrary.Functions;

public sealed class AddAlbum
{
    private readonly ISender _mediator;

    public AddAlbum(ISender mediator)
    {
        _mediator = mediator;
    }

    [Function(nameof(AddAlbum))]
    public async Task Run([RabbitMQTrigger("artist.album.added.queue", ConnectionStringSetting = "RabbitMq")] AlbumAddedEvent @event)
    {
        await _mediator.Send(new IngestAlbumAddedCommand { Event = @event });
    }
}
