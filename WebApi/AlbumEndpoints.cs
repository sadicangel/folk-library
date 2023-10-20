using DotNext;
using FluentValidation;
using MediatR;

namespace FolkLibrary.Infrastructure;

public static class AlbumEndpoints
{
    public static IEndpointRouteBuilder MapAlbumEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/albums");
        group.MapPost("/", CreateAlbum);
        group.MapPut("/{albumId}", UpdateAlbum);
        return endpoints;
    }

    private static Task<IResult> CreateAlbum(
        CreateAlbumCommand command,
        IValidator<CreateAlbumCommand> validator,
        IRequestHandler<CreateAlbumCommand, Result<Guid>> handler,
        CancellationToken cancellationToken)
    {
        return command.HandleAsync(validator, handler, cancellationToken);
    }

    private static Task<IResult> UpdateAlbum(
        Guid albumId,
        UpdateAlbumRequest request,
        IValidator<UpdateAlbumCommand> validator,
        IRequestHandler<UpdateAlbumCommand, Result<Unit>> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAlbumCommand(albumId, request);

        return command.HandleAsync(validator, handler, cancellationToken);
    }
}