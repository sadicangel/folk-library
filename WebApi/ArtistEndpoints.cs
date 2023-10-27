using DotNext;
using FolkLibrary.Artists;
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
        group.MapPut("/{artistId}", UpdateArtistInfo);
        group.MapPut("/{artistId}/{albumId}", AddArtistAlbum);
        group.MapDelete("/{artistId}/{albumId}", RemoveArtistAlbum);
        return endpoints;
    }

    private static Task<IResult> CreateArtist(
        CreateArtist command,
        IRequestHandler<CreateArtist, Result<Guid>> handler,
        CancellationToken cancellationToken)
    {
        return handler.Handle(command, cancellationToken).ToResultAsync();
    }

    private static Task<IResult> GetArtists(
        string? name,
        string? countryCode,
        string? countryName,
        string? district,
        string? municipality,
        string? parish,
        Param<int>? year,
        Param<int>? afterYear,
        Param<int>? beforeYear,
        Param<OrderBy>? sort,
        IRequestHandler<GetArtists, Result<GetArtistsResponse>> handler,
        CancellationToken cancellationToken)
    {
        var command = new GetArtists(name, countryCode, countryName, district, municipality, parish, year, afterYear, beforeYear, sort);

        return handler.Handle(command, cancellationToken).ToResultAsync();
    }

    private static Task<IResult> GetArtistById(
        Guid artistId,
        IRequestHandler<GetArtistById, Result<Optional<Artist>>> handler,
        CancellationToken cancellationToken)
    {
        var command = new GetArtistById(artistId);

        return handler.Handle(command, cancellationToken).ToResultAsync();
    }

    private static Task<IResult> UpdateArtistInfo(
        Guid artistId,
        UpdateArtistInfoRequest request,
        IRequestHandler<UpdateArtistInfo, Result<Unit>> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateArtistInfo(artistId, request);

        return handler.Handle(command, cancellationToken).ToResultAsync();
    }

    private static Task<IResult> AddArtistAlbum(
        Guid artistId,
        Guid albumId,
        IRequestHandler<AddArtistAlbum, Result<Unit>> handler,
        CancellationToken cancellationToken)
    {
        var command = new AddArtistAlbum(artistId, albumId);

        return handler.Handle(command, cancellationToken).ToResultAsync();
    }

    private static Task<IResult> RemoveArtistAlbum(
        Guid artistId,
        Guid albumId,
        IRequestHandler<RemoveArtistAlbum, Result<Unit>> handler,
        CancellationToken cancellationToken)
    {
        var command = new RemoveArtistAlbum(artistId, albumId);

        return handler.Handle(command, cancellationToken).ToResultAsync();
    }
}