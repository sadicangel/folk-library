using FolkLibrary.Dtos;
using FolkLibrary.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FolkLibrary.Functions;

public sealed class UpdateArtist
{
    private readonly IMongoRepository<ArtistDto> _artistRepository;

    public UpdateArtist(IMongoRepository<ArtistDto> artistRepository)
    {
        _artistRepository = artistRepository;
    }

    [FunctionName(nameof(UpdateArtist))]
    public async Task Run([RabbitMQTrigger("artist.updated.queue", ConnectionStringSetting = "RabbitMq")] ArtistDto artist, ILogger log)
    {
        await _artistRepository.UpdateAsync(artist);
        log.LogInformation($"Updated {artist.Name}");
    }
}
