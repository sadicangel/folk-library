using AutoMapper;
using FolkLibrary.Artists;
using FolkLibrary.Artists.Events;
using FolkLibrary.Repositories;
using Microsoft.Azure.Functions.Worker;

namespace FolkLibrary.Functions;

public sealed class IngestArtistCreated
{
    private readonly IMapper _mapper;
    private readonly IArtistRepository _artistRepository;
    private readonly IArtistViewRepository _artistViewRepository;

    public IngestArtistCreated(IMapper mapper, IArtistRepository artistRepository, IArtistViewRepository artistViewRepository)
    {
        _mapper = mapper;
        _artistRepository = artistRepository;
        _artistViewRepository = artistViewRepository;
    }

    [Function(nameof(IngestArtistCreated))]
    public async Task Run([RabbitMQTrigger("artist.created.queue", ConnectionStringSetting = "RabbitMq")] ArtistCreatedEvent @event, CancellationToken cancellationToken)
    {
        var artist = await _artistRepository.GetByIdAsync(@event.ArtistId, cancellationToken) ?? throw new FolkLibraryException($"{@event.ArtistId} not found");
        await _artistViewRepository.AddAsync(_mapper.Map<ArtistDto>(artist), cancellationToken);
    }
}