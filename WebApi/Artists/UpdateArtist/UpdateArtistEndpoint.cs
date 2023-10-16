using FastEndpoints;
using FolkLibrary.Artists.Events;
using FolkLibrary.Repositories;

namespace FolkLibrary.Artists.UpdateArtist;

public sealed class UpdateArtistEndpoint : Endpoint<UpdateArtistRequest, ArtistDto, UpdateArtistMapper>
{
    private readonly IArtistRepository _artistRepository;

    public UpdateArtistEndpoint(IArtistRepository artistRepository)
    {
        _artistRepository = artistRepository;
    }

    public override void Configure()
    {
        Put("/api/artist/{artistId}");
    }

    public override async Task HandleAsync(UpdateArtistRequest request, CancellationToken cancellationToken)
    {
        var artist = await Map.ToEntityAsync(request, cancellationToken);
        if (artist is null)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }

        await _artistRepository.UpdateAsync(artist, cancellationToken);

        await PublishAsync(new ArtistUpdatedEvent { ArtistId = artist.Id }, Mode.WaitForNone, cancellationToken);

        await SendMappedAsync(artist, 200, cancellationToken);
    }
}
