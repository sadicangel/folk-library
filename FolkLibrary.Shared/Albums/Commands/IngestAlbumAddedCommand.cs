using AutoMapper;
using FluentValidation;
using FolkLibrary.Albums.Events;
using FolkLibrary.Exceptions;
using FolkLibrary.Repositories;
using MediatR;

namespace FolkLibrary.Albums.Commands;
public sealed class IngestAlbumAddedCommand : IRequest
{
    public AlbumAddedEvent Event { get; init; } = null!;

    public sealed class Validator : AbstractValidator<IngestAlbumAddedCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Event).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<IngestAlbumAddedCommand>
    {
        private readonly IMapper _mapper;
        private readonly IAlbumEntityRepository _albumEntityRepository;
        private readonly IArtistDocumentRepository _artistDocumentRepository;

        public Handler(IMapper mapper, IAlbumEntityRepository albumEntityRepository, IArtistDocumentRepository artistDocumentRepository)
        {
            _mapper = mapper;
            _albumEntityRepository = albumEntityRepository;
            _artistDocumentRepository = artistDocumentRepository;
        }

        public async Task<Unit> Handle(IngestAlbumAddedCommand request, CancellationToken cancellationToken)
        {
            var artistDto = await _artistDocumentRepository.GetByIdAsync(request.Event.Data.ArtistId, cancellationToken);
            if (artistDto is null)
                throw new NotFoundException($"Artist {request.Event.Data.ArtistId} does not exist");

            var album = await _albumEntityRepository.GetByIdAsync(request.Event.Data.AlbumId, cancellationToken);
            if (album is null)
                throw new NotFoundException($"Album {request.Event.Data.AlbumId} does not exist");
            var albumDto = _mapper.Map<AlbumDocument>(album);

            artistDto.Albums.Add(albumDto);
            await _artistDocumentRepository.UpdateAsync(artistDto, cancellationToken);

            return Unit.Value;
        }
    }
}
