using DotNext;
using FolkLibrary.Artists;
using MediatR;
using System.Net;

namespace FolkLibrary.Infrastructure;

public static class ArtistEndpoints
{
    public static IEndpointRouteBuilder MapArtistEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/artists");

        group.MapPost("/", CreateArtist)
            .WithName("create-artist")
            .WithDisplayName($"Creates {nameof(Artist)}")
            .WithSummary($"Creates a new {nameof(Artist)}")
            .WithDescription($"Creates a new {nameof(Artist)}")
            .Produces<Guid>()
            .ProducesValidationProblem();

        group.MapGet("/", GetArtists)
            .WithName("get-artists")
            .WithDisplayName($"Gets {nameof(Artist)}s")
            .WithSummary($"Gets {nameof(Artist)}s")
            .WithDescription($"Gets {nameof(Artist)}s")
            .Produces<GetArtistsResponse>()
            .ProducesValidationProblem();

        group.MapGet("/{artistId}", GetArtistById)
            .WithName("get-artist")
            .WithDisplayName($"Get {nameof(Artist)}")
            .WithSummary($"Get {nameof(Artist)} by ID")
            .WithDescription($"Get {nameof(Artist)} by ID")
            .Produces<Artist>()
            .ProducesValidationProblem()
            .Produces((int)HttpStatusCode.NotFound);

        group.MapPut("/{artistId}", UpdateArtistInfo)
            .WithName("update-artist")
            .WithDisplayName($"Updates {nameof(Artist)}")
            .WithSummary($"Updates an {nameof(Artist)}")
            .WithDescription($"Updates an existing {nameof(Artist)}")
            .Produces((int)HttpStatusCode.OK)
            .ProducesValidationProblem()
            .Produces((int)HttpStatusCode.NotFound);

        group.MapPut("/{artistId}/{albumId}", AddArtistAlbum)
            .WithName("add-artist-album")
            .WithDisplayName($"Adds {nameof(Album)} to {nameof(Artist)}")
            .WithSummary($"Adds {nameof(Album)} to an {nameof(Artist)}")
            .WithDescription($"Adds {nameof(Album)} to an existing {nameof(Artist)}")
            .Produces((int)HttpStatusCode.OK)
            .ProducesValidationProblem()
            .Produces((int)HttpStatusCode.NotFound);

        group.MapDelete("/{artistId}/{albumId}", RemoveArtistAlbum)
            .WithName("remove-artist-album")
            .WithDisplayName($"Removes {nameof(Album)} from {nameof(Artist)}")
            .WithSummary($"Removes {nameof(Album)} from an {nameof(Artist)}")
            .WithDescription($"Removes {nameof(Album)} from an existing {nameof(Artist)}")
            .Produces((int)HttpStatusCode.OK)
            .ProducesValidationProblem()
            .Produces((int)HttpStatusCode.NotFound);

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