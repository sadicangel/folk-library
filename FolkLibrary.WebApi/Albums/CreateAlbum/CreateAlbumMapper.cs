using FastEndpoints;
using FolkLibrary.Repositories;
using FolkLibrary.Tracks;

namespace FolkLibrary.Albums.CreateAlbum;

public sealed partial class CreateAlbumMapper : Mapper<CreateAlbumRequest, AlbumDto, Album>
{
    public override Task<Album> ToEntityAsync(CreateAlbumRequest request, CancellationToken cancellationToken = default)
    {
        var album = new Album
        {
            Name = request.Name,
            Description = request.Description,
            Year = request.Year,
            IsYearUncertain = !request.Year.HasValue,
            Genres = request.Genres,
            Tracks = new HashSet<Track>(request.Tracks.Count)
        };

        foreach (var track in request.Tracks)
            album.Tracks.Add(track.ToTrack(album));

        return Resolve<IAlbumRepository>().AddAsync(album, cancellationToken);
    }

    public override Task<AlbumDto> FromEntityAsync(Album entity, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(entity.ToAlbumDto());
    }
}
