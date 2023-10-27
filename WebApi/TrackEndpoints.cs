using DotNext;
using FolkLibrary.Tracks;
using MediatR;

namespace FolkLibrary.Infrastructure;

public static class TrackEndpoints
{
    public static IEndpointRouteBuilder MapTrackEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/tracks");
        group.MapGet("/", GetTracks);
        group.MapGet("/{artistId}", GetTrackById);
        return endpoints;
    }

    private static Task<IResult> GetTracks(
        string? name,
        Param<int>? year,
        Param<int>? afterYear,
        Param<int>? beforeYear,
        Param<TimeSpan>? aboveDuration,
        Param<TimeSpan>? belowDuration,
        Param<OrderBy>? sort,
        IRequestHandler<GetTracks, Result<GetTracksResponse>> handler,
        CancellationToken cancellationToken)
    {
        var command = new GetTracks(name, year, afterYear, beforeYear, aboveDuration, belowDuration, sort);

        return handler.Handle(command, cancellationToken).ToResultAsync();
    }

    private static Task<IResult> GetTrackById(
        Guid trackId,
        IRequestHandler<GetTrackById, Result<Optional<Track>>> handler,
        CancellationToken cancellationToken)
    {
        var command = new GetTrackById(trackId);

        return handler.Handle(command, cancellationToken).ToResultAsync();
    }
}