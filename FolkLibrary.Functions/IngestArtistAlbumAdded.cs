using AutoMapper;
using FolkLibrary.Albums;
using FolkLibrary.Artists.Events;
using FolkLibrary.Repositories;
using Microsoft.Azure.Functions.Worker;

namespace FolkLibrary.Functions;

public sealed class IngestArtistAlbumAdded
{
    private readonly IMapper _mapper;
    private readonly IArtistViewRepository _artistViewRepository;
    private readonly IAlbumRepository _albumRepository;

    public IngestArtistAlbumAdded(IMapper mapper, IArtistViewRepository artistViewRepository, IAlbumRepository albumRepository)
    {
        _mapper = mapper;
        _artistViewRepository = artistViewRepository;
        _albumRepository = albumRepository;
    }

    [Function(nameof(IngestArtistAlbumAdded))]
    public async Task Run([RabbitMQTrigger("artist.album.added.queue", ConnectionStringSetting = "RabbitMq")] ArtistAlbumAddedEvent @event, CancellationToken cancellationToken)
    {
        var artistDto = await _artistViewRepository.GetByIdAsync(@event.ArtistId, cancellationToken) ?? throw new FolkLibraryException($"{@event.ArtistId} not found");
        var album = await _albumRepository.GetByIdAsync(@event.AlbumId, cancellationToken) ?? throw new FolkLibraryException($"{@event.AlbumId} not found");
        var albumDto = _mapper.Map<AlbumDto>(album);

        artistDto.Albums.Add(albumDto);
        await _artistViewRepository.UpdateAsync(artistDto, cancellationToken);
    }
}
