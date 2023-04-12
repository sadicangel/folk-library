using FluentValidation;
using FolkLibrary.Artists.Events;
using FolkLibrary.Errors;
using FolkLibrary.Repositories;
using FolkLibrary.Services;
using MediatR;
using OneOf.Types;

namespace FolkLibrary.Artists.Commands;

public sealed class AddArtistAlbumCommand : IRequest<Response<Success>>
{
    public required string ArtistId { get; init; }
    public required string AlbumId { get; init; }

    public List<int>? TracksContributed { get; init; }

    public sealed class Validator : AbstractValidator<AddArtistAlbumCommand>
    {
        public Validator()
        {
            RuleFor(e => e.ArtistId).NotEmpty();
            RuleFor(e => e.AlbumId).NotEmpty();
            When(e => e.TracksContributed is not null, () => RuleFor(e => e.TracksContributed).NotEmpty());
        }
    }

    public sealed class Handler : IRequestHandler<AddArtistAlbumCommand, Response<Success>>
    {
        private readonly IValidatorService<AddArtistAlbumCommand> _validator;
        private readonly IPublisher _mediator;
        private readonly IArtistRepository _artistRepository;
        private readonly IAlbumRepository _albumRepository;

        public Handler(IValidatorService<AddArtistAlbumCommand> validator, IPublisher mediator, IArtistRepository artistRepository, IAlbumRepository albumRepository)
        {
            _validator = validator;
            _mediator = mediator;
            _artistRepository = artistRepository;
            _albumRepository = albumRepository;
        }

        public async Task<Response<Success>> Handle(AddArtistAlbumCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var artist = await _artistRepository.GetByIdAsync(request.ArtistId, cancellationToken);
            if (artist is null)
                return new NotFound();
            var album = await _albumRepository.GetByIdAsync(request.AlbumId, cancellationToken);
            if (album is null)
                return new NotFound();
            artist.AlbumCount++;
            artist.Albums.Add(album);
            if (request.TracksContributed is null)
                artist.Tracks.AddRange(album.Tracks);
            else
            {
                artist.Tracks.AddRange(album.Tracks.Where(t => request.TracksContributed.Contains(t.Number)).ToList());
            }
            await _artistRepository.UpdateAsync(artist, cancellationToken);
            await _mediator.Publish(new ArtistAlbumAddedEvent
            {
                ArtistId = artist.Id,
                AlbumId = album.Id
            }, cancellationToken);

            return new Success();
        }
    }
}