using FolkLibrary.Dtos;
using FolkLibrary.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FolkLibrary.Functions;

public sealed class CreateArtist
{
    private readonly IMongoRepository<ArtistDto> _artistRepository;

    public CreateArtist(IMongoRepository<ArtistDto> artistRepository)
    {
        _artistRepository = artistRepository;
    }

    [FunctionName(nameof(CreateArtist))]
    public async Task Run([RabbitMQTrigger("artist.created.queue", ConnectionStringSetting = "RabbitMq")] ArtistDto artist, ILogger log)
    {
        await _artistRepository.AddAsync(artist);
        log.LogInformation($"Created {artist.Name}");
    }
}
