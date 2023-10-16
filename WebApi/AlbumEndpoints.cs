namespace FolkLibrary.Infrastructure;

public static class AlbumEndpoints
{
    public static IEndpointRouteBuilder MapAlbumEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/albums");
        group.MapPost("/", CreateAlbum);
        group.MapGet("/", GetAlbums);
        group.MapGet("/{albumId}", GetAlbumById);
        group.MapPut("/{albumId}", UpdateAlbum);
        return endpoints;
    }

    private static Task<IResult> CreateAlbum() => Task.FromResult(Results.Ok());

    private static Task<IResult> GetAlbums() => Task.FromResult(Results.Ok());

    private static Task<IResult> GetAlbumById() => Task.FromResult(Results.Ok());

    private static Task<IResult> UpdateAlbum() => Task.FromResult(Results.Ok());
}