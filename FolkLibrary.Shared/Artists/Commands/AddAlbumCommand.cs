using FluentValidation;
using FolkLibrary.Albums;
using FolkLibrary.Albums.Events;
using FolkLibrary.Exceptions;
using FolkLibrary.Repositories;
using MediatR;

namespace FolkLibrary.Artists.Commands;

public sealed class AddAlbumCommand : IRequest
{
    public ArtistId ArtistId { get; init; }
    public AlbumId AlbumId { get; init; }
    public List<int>? TracksContributed { get; init; }

    public sealed class Validator : AbstractValidator<AddAlbumCommand>
    {
        public Validator()
        {
            RuleFor(e => e.ArtistId).NotEmpty();
            RuleFor(e => e.AlbumId).NotEmpty();
            When(e => e.TracksContributed is not null, () => RuleFor(e => e.TracksContributed).NotEmpty());
        }
    }

    public sealed class Handler : IRequestHandler<AddAlbumCommand>
    {
        private readonly IPublisher _mediator;
        private readonly IArtistEntityRepository _artistEntityRepository;
        private readonly IAlbumEntityRepository _albumEntityRepository;

        public Handler(IPublisher mediator, IArtistEntityRepository artistEntityRepository, IAlbumEntityRepository albumEntityRepository)
        {
            _mediator = mediator;
            _artistEntityRepository = artistEntityRepository;
            _albumEntityRepository = albumEntityRepository;
        }

        public async Task<Unit> Handle(AddAlbumCommand request, CancellationToken cancellationToken)
        {
            var artist = await _artistEntityRepository.GetByIdAsync(request.ArtistId, cancellationToken);
            if (artist is null)
                throw new NotFoundException($"Artist {request.ArtistId} not found");
            var album = await _albumEntityRepository.GetByIdAsync(request.AlbumId, cancellationToken);
            if (album is null)
                throw new NotFoundException($"Album {request.AlbumId} not found");
            artist.AlbumCount++;
            artist.Albums.Add(album);
            if (request.TracksContributed is null)
                artist.Tracks.AddRange(album.Tracks);
            else
            {
                artist.Tracks.AddRange(album.Tracks.Where(t => request.TracksContributed.Contains(t.Number)).ToList());
            }
            await _artistEntityRepository.UpdateAsync(artist, cancellationToken);
            await _mediator.Publish(new AlbumAddedEvent
            {
                Data = new AlbumAddedEventData
                {
                    ArtistId = artist.Id,
                    AlbumId = album.Id
                }
            }, cancellationToken);
            return Unit.Value;
        }
    }
}