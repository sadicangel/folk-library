using AutoMapper;
using FluentValidation;
using FolkLibrary.Albums;
using FolkLibrary.Artists.Events;
using FolkLibrary.Errors;
using FolkLibrary.Repositories;
using FolkLibrary.Services;
using MediatR;
using OneOf.Types;

namespace FolkLibrary.Artists.Commands;
public sealed class IngestArtistAlbumAddedEventCommand : IRequest<Response<Success>>
{
    public required ArtistAlbumAddedEvent Event { get; init; }

    public sealed class Validator : AbstractValidator<IngestArtistAlbumAddedEventCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Event).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<IngestArtistAlbumAddedEventCommand, Response<Success>>
    {
        private readonly IValidatorService<IngestArtistAlbumAddedEventCommand> _validator;
        private readonly IMapper _mapper;
        private readonly IAlbumRepository _albumEntityRepository;
        private readonly IArtistViewRepository _artistViewRepository;

        public Handler(IValidatorService<IngestArtistAlbumAddedEventCommand> validator, IMapper mapper, IAlbumRepository albumEntityRepository, IArtistViewRepository artistViewRepository)
        {
            _validator = validator;
            _mapper = mapper;
            _albumEntityRepository = albumEntityRepository;
            _artistViewRepository = artistViewRepository;
        }

        public async Task<Response<Success>> Handle(IngestArtistAlbumAddedEventCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var artistDto = await _artistViewRepository.GetByIdAsync(request.Event.ArtistId, cancellationToken);
            if (artistDto is null)
                return new NotFound();

            var album = await _albumEntityRepository.GetByIdAsync(request.Event.AlbumId, cancellationToken);
            if (album is null)
                return new NotFound();
            var albumDto = _mapper.Map<AlbumDto>(album);

            artistDto.Albums.Add(albumDto);
            await _artistViewRepository.UpdateAsync(artistDto, cancellationToken);

            return new Success();
        }
    }
}
