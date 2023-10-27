using DotNext;
using FolkLibrary.Albums;
using MediatR;

namespace FolkLibrary.Infrastructure;

public static class AlbumEndpoints
{
    public static IEndpointRouteBuilder MapAlbumEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/albums");
        group.MapPost("/", CreateAlbum);
        group.MapPut("/{albumId}", UpdateAlbumInfo);
        group.MapPut("/{albumId}/{trackId}", AddAlbumTrack);
        return endpoints;
    }

    private static Task<IResult> CreateAlbum(CreateAlbum command, IRequestHandler<CreateAlbum, Result<Guid>> handler, CancellationToken cancellationToken) =>
        handler.Handle(command, cancellationToken).ToResultAsync();

    private static Task<IResult> UpdateAlbumInfo(
        Guid albumId,
        UpdateAlbumInfoRequest request,
        IRequestHandler<UpdateAlbumInfo, Result<Unit>> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAlbumInfo(albumId, request);

        return handler.Handle(command, cancellationToken).ToResultAsync();
    }

    public static Task<IResult> AddAlbumTrack(
        Guid albumId,
        Guid trackId,
        IRequestHandler<AddAlbumTrack, Result<Unit>> handler,
        CancellationToken cancellationToken)
    {
        var command = new AddAlbumTrack(albumId, trackId);

        return handler.Handle(command, cancellationToken).ToResultAsync();
    }
}