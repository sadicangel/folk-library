using Ardalis.Specification;
using FastEndpoints;
using FluentValidation;
using FolkLibrary.Albums.Events;
using FolkLibrary.Albums.GetAlbumById;
using FolkLibrary.Artists;
using FolkLibrary.Repositories;

namespace FolkLibrary.Albums.CreateCompilationAlbum;

public sealed class CreateCompilationAlbumEndpoint : Endpoint<CreateCompilationAlbumRequest, AlbumDto, CreateCompilationAlbumMapper>
{
    private readonly IArtistRepository _artistRepository;

    public CreateCompilationAlbumEndpoint(IArtistRepository artistRepository)
    {
        _artistRepository = artistRepository;
    }

    public override void Configure()
    {
        Post("/api/album/compilation");
    }

    public override async Task HandleAsync(CreateCompilationAlbumRequest request, CancellationToken cancellationToken)
    {
        var artists = await _artistRepository.ListAsync(new FindArtistsByIdSpecification(request.TracksByArtistId.Keys), cancellationToken);

        if (artists.Count != request.TracksByArtistId.Count)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }

        var album = await Map.ToEntityAsync(request, cancellationToken);

        foreach (var artist in artists)
        {
            artist.Albums.Add(album);
            var trackNumbers = request.TracksByArtistId[artist.Id];
            artist.Tracks.UnionWith(album.Tracks.Where(t => trackNumbers.Contains(t.Number)));
        }

        await _artistRepository.UpdateRangeAsync(artists, cancellationToken);
        await PublishAsync(new CompilationAlbumCreatedEvent { TracksByArtistId = request.TracksByArtistId, AlbumId = album.Id }, Mode.WaitForNone, cancellationToken);


        await SendCreatedAtAsync<GetAlbumByIdEndpoint>(new { albumId = album.Id }, Map.FromEntity(album), cancellation: cancellationToken);
    }

    private sealed class FindArtistsByIdSpecification : Specification<Artist>
    {
        public FindArtistsByIdSpecification(ICollection<string> artistIds)
        {
            Query.Where(a => artistIds.Contains(a.Id));
        }
    }
}