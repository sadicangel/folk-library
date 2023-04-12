using FolkLibrary.Artists.Commands;
using FolkLibrary.Artists.Events;
using MediatR;
using Microsoft.Azure.Functions.Worker;

namespace FolkLibrary.Functions;

public sealed class DeleteArtist
{
    private readonly ISender _mediator;

    public DeleteArtist(ISender mediator)
    {
        _mediator = mediator;
    }

    [Function(nameof(DeleteArtist))]
    public async Task Run([RabbitMQTrigger("artist.deleted.queue", ConnectionStringSetting = "RabbitMq")] ArtistDeletedEvent @event)
    {
        await _mediator.Send(new IngestArtistDeletedEventCommand { Event = @event });
    }
}
