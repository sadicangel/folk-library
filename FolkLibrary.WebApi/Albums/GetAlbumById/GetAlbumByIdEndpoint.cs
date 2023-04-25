using FastEndpoints;

namespace FolkLibrary.Albums.GetAlbumById;

public sealed class GetAlbumByIdEndpoint : Endpoint<GetAlbumByIdRequest, AlbumDto, GetAlbumByIdMapper>
{
    public override void Configure()
    {
        Get("/api/album/{albumId}");
    }

    public override async Task HandleAsync(GetAlbumByIdRequest request, CancellationToken cancellationToken)
    {
        var album = await Map.ToEntityAsync(request, cancellationToken);
        if (album is null)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }
        var albumDto = await Map.FromEntityAsync(album, cancellationToken);
        await SendOkAsync(albumDto, cancellationToken);
    }
}