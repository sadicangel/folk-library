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
        group.MapDelete("/{albumId}", DeleteAlbum);
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
        List<Guid> artistId,
        UpdateAlbumRequest request,
        IValidator<UpdateAlbumCommand> validator,
        IRequestHandler<UpdateAlbumCommand, Result<Unit>> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAlbumCommand(albumId, artistId, request);

        return command.HandleAsync(validator, handler, cancellationToken);
    }


    private static Task<IResult> DeleteAlbum(
        Guid albumId,
        List<Guid> artistId,
        IValidator<DeleteAlbumCommand> validator,
        IRequestHandler<DeleteAlbumCommand, Result<Unit>> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteAlbumCommand(albumId, artistId);

        return command.HandleAsync(validator, handler, cancellationToken);
    }
}