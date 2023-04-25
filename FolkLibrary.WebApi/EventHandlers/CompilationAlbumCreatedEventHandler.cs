using Ardalis.Specification;
using FastEndpoints;
using FolkLibrary.Albums;
using FolkLibrary.Albums.Events;
using FolkLibrary.Artists;
using FolkLibrary.Repositories;
using Mapster;

namespace FolkLibrary.EventHandlers;

public sealed class CompilationAlbumCreatedEventHandler : IEventHandler<CompilationAlbumCreatedEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CompilationAlbumCreatedEventHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task HandleAsync(CompilationAlbumCreatedEvent @event, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var artistViewRepository = scope.Resolve<IArtistViewRepository>();
        var albumRepository = scope.Resolve<IAlbumRepository>();

        var artistDtos = await artistViewRepository.ListAsync(new FindArtistsByIdSpecification(@event.TracksByArtistId.Keys), cancellationToken);
        if (artistDtos.Count != @event.TracksByArtistId.Keys.Count)
            throw new FolkLibraryException($"Artists missing: {String.Join(", ", @event.TracksByArtistId.Keys.Where(id => artistDtos.FindIndex(a => a.Id == id) < 0))}");

        var album = await albumRepository.GetByIdAsync(@event.AlbumId, cancellationToken) ?? throw new FolkLibraryException($"{@event.AlbumId} not found");

        artistDtos.ForEach(artist =>
        {
            var albumDto = album.Adapt<AlbumDto>();
            albumDto.IsCompilation = true;
            albumDto.TracksContributedByArtist = @event.TracksByArtistId[artist.Id];
            artist.Albums.Add(albumDto);
        });
        await artistViewRepository.UpdateRangeAsync(artistDtos, cancellationToken);
    }

    private sealed class FindArtistsByIdSpecification : Specification<ArtistDto>
    {
        public FindArtistsByIdSpecification(ICollection<string> artistIds)
        {
            Query.Where(a => artistIds.Contains(a.Id));
        }
    }
}
