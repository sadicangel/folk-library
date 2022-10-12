using FolkLibrary.Dtos;
using FolkLibrary.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FolkLibrary.Functions;

public sealed class CreateArtist
{
    private readonly ILogger<CreateArtist> _logger;
    private readonly IMongoRepository<ArtistDto> _artistRepository;

    public CreateArtist(ILogger<CreateArtist> logger, IMongoRepository<ArtistDto> artistRepository)
    {
        _logger = logger;
        _artistRepository = artistRepository;
    }

    [Function(nameof(CreateArtist))]
    public async Task Run([RabbitMQTrigger("artist.created.queue", ConnectionStringSetting = "RabbitMq")] ArtistDto artist)
    {
        await _artistRepository.AddAsync(artist);
        _logger.LogInformation("Created {artistName}", artist.Name);
    }
}
