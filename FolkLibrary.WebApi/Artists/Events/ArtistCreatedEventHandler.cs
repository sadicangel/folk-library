using FastEndpoints;
using FolkLibrary.Repositories;
using IMapper = AutoMapper.IMapper;

namespace FolkLibrary.Artists.Events;
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
        var mapper = scope.Resolve<IMapper>();
        var artist = await artistRepository.GetByIdAsync(@event.ArtistId, cancellationToken) ?? throw new FolkLibraryException($"{@event.ArtistId} not found");
        await artistViewRepository.AddAsync(mapper.Map<ArtistDto>(artist), cancellationToken);
    }
}
