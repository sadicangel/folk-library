using FolkLibrary.Dtos;
using FolkLibrary.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FolkLibrary.Functions;

public sealed class UpdateArtist
{
    private readonly ILogger<UpdateArtist> _logger;
    private readonly IMongoRepository<ArtistDto> _artistRepository;

    public UpdateArtist(ILogger<UpdateArtist> logger, IMongoRepository<ArtistDto> artistRepository)
    {
        _logger = logger;
        _artistRepository = artistRepository;
    }

    [Function(nameof(UpdateArtist))]
    public async Task Run([RabbitMQTrigger("artist.updated.queue", ConnectionStringSetting = "RabbitMq")] ArtistDto artist)
    {
        await _artistRepository.UpdateAsync(artist);
        _logger.LogInformation("Updated {artistName}", artist.Name);
    }
}
