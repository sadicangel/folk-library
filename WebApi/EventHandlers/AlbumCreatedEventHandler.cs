using FastEndpoints;
using FolkLibrary.Albums.Events;
using FolkLibrary.Repositories;

namespace FolkLibrary.EventHandlers;
public sealed class AlbumCreatedEventHandler : IEventHandler<AlbumCreatedEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AlbumCreatedEventHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task HandleAsync(AlbumCreatedEvent @event, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var artistViewRepository = scope.Resolve<IArtistViewRepository>();
        var albumRepository = scope.Resolve<IAlbumRepository>();

        var artistDto = await artistViewRepository.GetByIdAsync(@event.ArtistId, cancellationToken) ?? throw new FolkLibraryException($"{@event.ArtistId} not found");
        var album = await albumRepository.GetByIdAsync(@event.AlbumId, cancellationToken) ?? throw new FolkLibraryException($"{@event.AlbumId} not found");
        var albumDto = album.ToAlbumDto();

        artistDto.Albums.Add(albumDto);
        await artistViewRepository.UpdateAsync(artistDto, cancellationToken);
    }
}