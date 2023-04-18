using FastEndpoints;
using FluentValidation;
using FolkLibrary.Artists.Events;
using FolkLibrary.Repositories;

namespace FolkLibrary.Endpoints;

public sealed class AddArtistAlbumRequest
{
    public required string ArtistId { get; init; }
    public required string AlbumId { get; init; }

    [FromBody]
    public required List<int> Tracks { get; init; }

    public sealed class Validator : Validator<AddArtistAlbumRequest>
    {
        public Validator()
        {
            RuleFor(e => e.ArtistId).NotEmpty();
            RuleFor(e => e.AlbumId).NotEmpty();
        }
    }
}

public sealed class AddArtistAlbumEndpoint : Endpoint<AddArtistAlbumRequest>
{
    private readonly IArtistRepository _artistRepository;
    private readonly IAlbumRepository _albumRepository;

    public AddArtistAlbumEndpoint(IArtistRepository artistRepository, IAlbumRepository albumRepository)
    {
        _artistRepository = artistRepository;
        _albumRepository = albumRepository;
        _albumRepository = albumRepository;
        _artistRepository = artistRepository;
    }

    public override void Configure()
    {
        Put("/api/artist/{artistId}/album/{albumId}");
    }

    public override async Task HandleAsync(AddArtistAlbumRequest request, CancellationToken cancellationToken)
    {
        var artist = await _artistRepository.GetByIdAsync(request.ArtistId, cancellationToken);
        if (artist is null)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }
        var album = await _albumRepository.GetByIdAsync(request.AlbumId, cancellationToken);
        if (album is null)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }
        artist.Albums.Add(album);
        if (request.Tracks.Count > 0)
            artist.Tracks.UnionWith(album.Tracks.Where(t => request.Tracks.Contains(t.Number)).ToList());
        else
            artist.Tracks.UnionWith(album.Tracks);

        await _artistRepository.UpdateAsync(artist, cancellationToken);
        await PublishAsync(new ArtistAlbumAddedEvent { ArtistId = artist.Id, AlbumId = album.Id }, Mode.WaitForNone, cancellationToken);

        await SendOkAsync(cancellationToken);
    }
}