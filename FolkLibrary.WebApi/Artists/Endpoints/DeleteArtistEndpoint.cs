using FastEndpoints;
using FluentValidation;
using FolkLibrary.Repositories;

namespace FolkLibrary.Endpoints;

public sealed class DeleteArtistRequest
{
    public required string ArtistId { get; init; }

    public sealed class Validator : Validator<DeleteArtistRequest>
    {
        public Validator()
        {
            RuleFor(e => e.ArtistId).NotEmpty();
        }
    }
}

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

        await SendOkAsync(cancellationToken);
    }
}