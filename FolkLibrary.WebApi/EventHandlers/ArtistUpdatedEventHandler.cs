using FastEndpoints;
using FolkLibrary.Artists;
using FolkLibrary.Artists.Events;
using FolkLibrary.Repositories;
using Mapster;

namespace FolkLibrary.EventHandlers;

public sealed class ArtistUpdatedEventHandler : IEventHandler<ArtistUpdatedEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ArtistUpdatedEventHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task HandleAsync(ArtistUpdatedEvent @event, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var artistRepository = scope.Resolve<IArtistRepository>();
        var artistViewRepository = scope.Resolve<IArtistViewRepository>();
        var artist = await artistRepository.GetByIdAsync(@event.ArtistId, cancellationToken) ?? throw new FolkLibraryException($"{@event.ArtistId} not found");
        await artistViewRepository.UpdateAsync(artist.Adapt<ArtistDto>(), cancellationToken);
    }
}
