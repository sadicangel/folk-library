using FolkLibrary.Dtos;
using FolkLibrary.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FolkLibrary.Functions;

public sealed class DeleteArtist
{
    private readonly ILogger<DeleteArtist> _logger;
    private readonly IMongoRepository<ArtistDto> _artistRepository;

    public DeleteArtist(ILogger<DeleteArtist> logger, IMongoRepository<ArtistDto> artistRepository)
    {
        _logger = logger;
        _artistRepository = artistRepository;
    }

    [Function(nameof(DeleteArtist))]
    public async Task Run([RabbitMQTrigger("artist.deleted.queue", ConnectionStringSetting = "RabbitMq")] ArtistDto artist)
    {
        await _artistRepository.DeleteAsync(artist);
        _logger.LogInformation("Deleted {artistName}", artist.Name);
    }
}
