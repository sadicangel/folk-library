using DotNext;
using FluentValidation;
using MediatR;

namespace FolkLibrary.Infrastructure;

public static class ArtistEndpoints
{
    public static IEndpointRouteBuilder MapArtistEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/artists");
        group.MapPost("/", CreateArtist);
        group.MapGet("/", GetArtists);
        group.MapGet("/{artistId}", GetArtistById);
        group.MapPut("/{artistId}", UpdateArtist);
        return endpoints;
    }

    private static Task<IResult> CreateArtist(
        CreateArtistCommand command,
        IValidator<CreateArtistCommand> validator,
        IRequestHandler<CreateArtistCommand, Result<Guid>> handler,
        CancellationToken cancellationToken)
    {
        return command.HandleAsync(validator, handler, cancellationToken);
    }

    private static Task<IResult> GetArtists(
        string? country,
        string? district,
        string? municipality,
        string? parish,
        int? afterYear,
        int? beforeYear,
        IValidator<GetArtistsCommand> validator,
        IRequestHandler<GetArtistsCommand, Result<GetArtistsResponse>> handler,
        CancellationToken cancellationToken)
    {
        var command = new GetArtistsCommand(country, district, municipality, parish, afterYear, beforeYear);

        return command.HandleAsync(validator, handler, cancellationToken);
    }

    private static Task<IResult> GetArtistById(
        Guid artistId,
        IValidator<GetArtistByIdCommand> validator,
        IRequestHandler<GetArtistByIdCommand, Result<Optional<Artist>>> handler,
        CancellationToken cancellationToken)
    {
        var command = new GetArtistByIdCommand(artistId);

        return command.HandleAsync(validator, handler, cancellationToken);
    }

    private static Task<IResult> UpdateArtist(
        Guid artistId,
        UpdateArtistRequest request,
        IValidator<UpdateArtistCommand> validator,
        IRequestHandler<UpdateArtistCommand, Result<Unit>> handler,
        CancellationToken cancellationToken
        )
    {
        var command = new UpdateArtistCommand(artistId, request);

        return command.HandleAsync(validator, handler, cancellationToken);
    }
}