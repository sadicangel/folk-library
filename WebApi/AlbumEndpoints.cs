using DotNext;
using FolkLibrary.Albums;
using MediatR;
using System.Net;

namespace FolkLibrary.Infrastructure;

public static class AlbumEndpoints
{
    public static IEndpointRouteBuilder MapAlbumEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/albums");

        group.MapPost("/", CreateAlbum)
            .WithName("create-album")
            .WithDisplayName($"Creates {nameof(Album)}")
            .WithSummary($"Creates a new {nameof(Album)}")
            .WithDescription($"Creates a new {nameof(Album)}")
            .Produces<Guid>()
            .ProducesValidationProblem();

        group.MapGet("/", GetAlbums)
            .WithName("get-albums")
            .WithDisplayName($"Gets {nameof(Album)}s")
            .WithSummary($"Gets {nameof(Album)}s")
            .WithDescription($"Gets {nameof(Album)}s")
            .Produces<GetAlbumsResponse>()
            .ProducesValidationProblem();

        group.MapGet("/{artistId}", GetAlbumById)
            .WithName("get-album")
            .WithDisplayName($"Get {nameof(Album)}")
            .WithSummary($"Get {nameof(Album)} by ID")
            .WithDescription($"Get {nameof(Album)} by ID")
            .Produces<Album>()
            .ProducesValidationProblem()
            .Produces((int)HttpStatusCode.NotFound);

        group.MapPut("/{albumId}", UpdateAlbumInfo)
            .WithName("update-album")
            .WithDisplayName($"Updates {nameof(Album)}")
            .WithSummary($"Updates an {nameof(Album)}")
            .WithDescription($"Updates an existing {nameof(Album)}")
            .Produces((int)HttpStatusCode.OK)
            .ProducesValidationProblem()
            .Produces((int)HttpStatusCode.NotFound);

        group.MapPut("/{albumId}/{trackId}", AddAlbumTrack)
            .WithName("add-album-track")
            .WithDisplayName($"Adds {nameof(Track)} to {nameof(Album)}")
            .WithSummary($"Adds {nameof(Track)} to an {nameof(Album)}")
            .WithDescription($"Adds {nameof(Track)} to an existing {nameof(Album)}")
            .Produces((int)HttpStatusCode.OK)
            .ProducesValidationProblem()
            .Produces((int)HttpStatusCode.NotFound);

        return endpoints;
    }

    private static Task<IResult> CreateAlbum(CreateAlbum command, IRequestHandler<CreateAlbum, Result<Guid>> handler, CancellationToken cancellationToken) =>
        handler.Handle(command, cancellationToken).ToResultAsync();

    private static Task<IResult> GetAlbums(
        string? name,
        Param<int>? year,
        Param<int>? afterYear,
        Param<int>? beforeYear,
        Param<OrderBy>? sort,
        IRequestHandler<GetAlbums, Result<GetAlbumsResponse>> handler,
        CancellationToken cancellationToken)
    {
        var command = new GetAlbums(name, year, afterYear, beforeYear, sort);

        return handler.Handle(command, cancellationToken).ToResultAsync();
    }

    private static Task<IResult> GetAlbumById(
        Guid albumId,
        IRequestHandler<GetAlbumById, Result<Optional<Album>>> handler,
        CancellationToken cancellationToken)
    {
        var command = new GetAlbumById(albumId);

        return handler.Handle(command, cancellationToken).ToResultAsync();
    }

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
