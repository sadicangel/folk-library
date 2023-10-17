using DotNext;
using MediatR;

namespace FolkLibrary.Infrastructure;

public static class ArtistEndpoints
{
    public static IEndpointRouteBuilder MapArtistEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/artists");
        group.MapPost("/", Handle<CreateArtistRequest>);
        group.MapGet("/", GetArtists);
        group.MapGet("/{artistId}", GetArtistById);
        group.MapPut("/{artistId}", UpdateArtist);
        return endpoints;
    }

    private static async Task<IResult> Handle<TRequest>(TRequest request, IRequestHandler<TRequest, Result<Unit>> handler, CancellationToken cancellationToken)
        where TRequest : IRequest<Result<Unit>>
    {
        var result = await handler.Handle(request, cancellationToken);
        return result.Match(
            ok => Results.Ok(),
            err => Results.BadRequest(err));

    }

    private static async Task<IResult> Handle<TRequest, TResponse>(TRequest request, IRequestHandler<TRequest, Result<TResponse>> handler, CancellationToken cancellationToken)
        where TRequest : IRequest<Result<TResponse>>
    {
        await handler.Handle(request, cancellationToken);
        return Results.Ok(request);
    }

    private static Task<IResult> GetArtists() => Task.FromResult(Results.Ok());

    private static Task<IResult> GetArtistById() => Task.FromResult(Results.Ok());

    private static Task<IResult> UpdateArtist() => Task.FromResult(Results.Ok());
}
