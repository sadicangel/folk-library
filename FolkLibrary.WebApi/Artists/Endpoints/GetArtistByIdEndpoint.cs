using FastEndpoints;
using FluentValidation;
using FolkLibrary.Artists;
using FolkLibrary.Repositories;

namespace FolkLibrary.Endpoints;

public sealed class GetArtistByIdRequest
{
    public required string ArtistId { get; init; }

    public sealed class Validator : Validator<GetArtistByIdRequest>
    {
        public Validator() => RuleFor(e => e.ArtistId).NotEmpty();
    }
}

public sealed class GetArtistByIdEndpoint : Endpoint<GetArtistByIdRequest, ArtistDto>
{
    private readonly IArtistViewRepository _artistViewRepository;

    public GetArtistByIdEndpoint(IArtistViewRepository artistViewRepository)
    {
        _artistViewRepository = artistViewRepository;
    }

    public override void Configure()
    {
        Get("/api/artist/{artistId}");
    }

    public override async Task HandleAsync(GetArtistByIdRequest request, CancellationToken cancellationToken)
    {
        var artist = await _artistViewRepository.GetByIdAsync(request.ArtistId, cancellationToken);
        if (artist is null)
            await SendNotFoundAsync(cancellationToken);
        else
            await SendOkAsync(artist, cancellationToken);
    }
}