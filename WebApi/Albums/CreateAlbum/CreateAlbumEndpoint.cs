using FastEndpoints;
using FolkLibrary.Albums.Events;
using FolkLibrary.Albums.GetAlbumById;
using FolkLibrary.Repositories;

namespace FolkLibrary.Albums.CreateAlbum;

public sealed class CreateAlbumEndpoint : Endpoint<CreateAlbumRequest, AlbumDto, CreateAlbumMapper>
{
    private readonly IArtistRepository _artistRepository;

    public CreateAlbumEndpoint(IArtistRepository artistRepository)
    {
        _artistRepository = artistRepository;
    }

    public override void Configure()
    {
        Post("/api/album");
    }

    public override async Task HandleAsync(CreateAlbumRequest request, CancellationToken cancellationToken)
    {
        var artist = await _artistRepository.GetByIdAsync(request.ArtistId, cancellationToken);
        if (artist is null)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }

        var album = await Map.ToEntityAsync(request, cancellationToken);

        artist.Albums.Add(album);
        artist.Tracks.UnionWith(album.Tracks);

        await _artistRepository.UpdateAsync(artist, cancellationToken);
        await PublishAsync(new AlbumCreatedEvent { ArtistId = artist.Id, AlbumId = album.Id }, Mode.WaitForNone, cancellationToken);

        await SendCreatedAtAsync<GetAlbumByIdEndpoint>(new { albumId = album.Id }, Map.FromEntity(album), cancellation: cancellationToken);
    }
}