using FolkLibrary.Artists.Commands;
using FolkLibrary.Artists.Events;
using MediatR;
using Microsoft.Azure.Functions.Worker;

namespace FolkLibrary.Functions;

public sealed class CreateArtist
{
    private readonly ISender _mediator;

    public CreateArtist(ISender mediator)
    {
        _mediator = mediator;
    }

    [Function(nameof(CreateArtist))]
    public async Task Run([RabbitMQTrigger("artist.created.queue", ConnectionStringSetting = "RabbitMq")] ArtistCreatedEvent @event)
    {
        await _mediator.Send(new IngestCreatedArtistCommand { Event = @event });
    }
}
