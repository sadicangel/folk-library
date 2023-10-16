using FastEndpoints;
using FolkLibrary.Artists.Events;
using FolkLibrary.Repositories;

namespace FolkLibrary.Artists.DeleteArtist;

public sealed class DeleteArtistEndpoint : Endpoint<DeleteArtistRequest>
{
    private readonly IArtistRepository _artistRepository;

    public DeleteArtistEndpoint(IArtistRepository artistRepository)
    {
        _artistRepository = artistRepository;
    }

    public override void Configure()
    {
        Delete("/api/artist/{artistId}");
    }

    public override async Task HandleAsync(DeleteArtistRequest request, CancellationToken cancellationToken)
    {
        var artist = await _artistRepository.GetByIdAsync(request.ArtistId, cancellationToken);
        if (artist is null)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }

        await _artistRepository.DeleteAsync(artist, cancellationToken);

        await PublishAsync(new ArtistDeletedEvent { ArtistId = request.ArtistId }, Mode.WaitForNone, cancellationToken);

        await SendOkAsync(cancellationToken);
    }
}