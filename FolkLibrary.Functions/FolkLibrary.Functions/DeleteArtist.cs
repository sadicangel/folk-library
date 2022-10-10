using FolkLibrary.Dtos;
using FolkLibrary.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FolkLibrary.Functions;

public sealed class DeleteArtist
{
    private readonly IMongoRepository<ArtistDto> _artistRepository;

    public DeleteArtist(IMongoRepository<ArtistDto> artistRepository)
    {
        _artistRepository = artistRepository;
    }

    [FunctionName(nameof(DeleteArtist))]
    public async Task Run([RabbitMQTrigger("artist.deleted.queue", ConnectionStringSetting = "RabbitMq")] ArtistDto artist, ILogger log)
    {
        await _artistRepository.DeleteAsync(artist);
        log.LogInformation($"Deleted {artist.Name}");
    }
}
