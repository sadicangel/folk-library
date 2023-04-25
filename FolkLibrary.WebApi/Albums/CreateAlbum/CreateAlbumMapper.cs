using FastEndpoints;
using FolkLibrary.Repositories;
using Mapster;

namespace FolkLibrary.Albums.CreateAlbum;

public sealed class CreateAlbumMapper : Mapper<CreateAlbumRequest, AlbumDto, Album>
{
    public override Task<Album> ToEntityAsync(CreateAlbumRequest request, CancellationToken cancellationToken = default)
    {
        var album = request.Adapt<Album>();
        album.IsYearUncertain = !album.Year.HasValue;
        foreach (var track in album.Tracks)
            track.IsYearUncertain = !track.Year.HasValue;

        return Resolve<IAlbumRepository>().AddAsync(album, cancellationToken);
    }

    public override Task<AlbumDto> FromEntityAsync(Album entity, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(entity.Adapt<AlbumDto>());
    }
}
