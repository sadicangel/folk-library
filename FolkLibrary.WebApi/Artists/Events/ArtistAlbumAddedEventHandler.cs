using FastEndpoints;
using FolkLibrary.Albums;
using FolkLibrary.Repositories;
using IMapper = AutoMapper.IMapper;

namespace FolkLibrary.Artists.Events;
public sealed class ArtistAlbumAddedEventHandler : IEventHandler<ArtistAlbumAddedEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ArtistAlbumAddedEventHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task HandleAsync(ArtistAlbumAddedEvent @event, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var artistViewRepository = scope.Resolve<IArtistViewRepository>();
        var albumRepository = scope.Resolve<IAlbumRepository>();
        var mapper = scope.Resolve<IMapper>();

        var artistDto = await artistViewRepository.GetByIdAsync(@event.ArtistId, cancellationToken) ?? throw new FolkLibraryException($"{@event.ArtistId} not found");
        var album = await albumRepository.GetByIdAsync(@event.AlbumId, cancellationToken) ?? throw new FolkLibraryException($"{@event.AlbumId} not found");
        var albumDto = mapper.Map<AlbumDto>(album);

        artistDto.Albums.Add(albumDto);
        await artistViewRepository.UpdateAsync(artistDto, cancellationToken);
    }
}
