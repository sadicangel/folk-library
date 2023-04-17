using FastEndpoints;
using FolkLibrary.Repositories;

namespace FolkLibrary.Artists.Events;

public sealed class ArtistDeletedEventHandler : IEventHandler<ArtistDeletedEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ArtistDeletedEventHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task HandleAsync(ArtistDeletedEvent @event, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var _artistViewRepository = scope.Resolve<IArtistViewRepository>();
        var artist = await _artistViewRepository.GetByIdAsync(@event.Id, cancellationToken) ?? throw new FolkLibraryException($"{@event.ArtistId} not found");
        await _artistViewRepository.DeleteAsync(artist, cancellationToken);
    }
}
