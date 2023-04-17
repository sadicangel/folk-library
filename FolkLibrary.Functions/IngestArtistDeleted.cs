using FolkLibrary.Artists.Events;
using FolkLibrary.Repositories;
using Microsoft.Azure.Functions.Worker;

namespace FolkLibrary.Functions;

public sealed class IngestArtistDeleted
{
    private readonly IArtistViewRepository _artistViewRepository;

    public IngestArtistDeleted(IArtistViewRepository artistViewRepository)
    {
        _artistViewRepository = artistViewRepository;
    }

    [Function(nameof(IngestArtistDeleted))]
    public async Task Run([RabbitMQTrigger("artist.deleted.queue", ConnectionStringSetting = "RabbitMq")] ArtistDeletedEvent @event, CancellationToken cancellationToken)
    {
        var artist = await _artistViewRepository.GetByIdAsync(@event.Id, cancellationToken) ?? throw new FolkLibraryException($"{@event.ArtistId} not found");
        await _artistViewRepository.DeleteAsync(artist, cancellationToken);
    }
}
