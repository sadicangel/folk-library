using FastEndpoints;
using FolkLibrary.Artists.Events;
using FolkLibrary.Repositories;

namespace FolkLibrary.EventHandlers;
public sealed class ArtistCreatedEventHandler : IEventHandler<ArtistCreatedEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ArtistCreatedEventHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task HandleAsync(ArtistCreatedEvent @event, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var artistRepository = scope.Resolve<IArtistRepository>();
        var artistViewRepository = scope.Resolve<IArtistViewRepository>();
        var artist = await artistRepository.GetByIdAsync(@event.ArtistId, cancellationToken) ?? throw new FolkLibraryException($"{@event.ArtistId} not found");
        await artistViewRepository.AddAsync(artist.ToArtistDto(), cancellationToken);
    }
}
