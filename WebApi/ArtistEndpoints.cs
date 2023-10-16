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

    private static Task<IResult> CreateArtist() => Task.FromResult(Results.Ok());

    private static Task<IResult> GetArtists() => Task.FromResult(Results.Ok());

    private static Task<IResult> GetArtistById() => Task.FromResult(Results.Ok());

    private static Task<IResult> UpdateArtist() => Task.FromResult(Results.Ok());
}
