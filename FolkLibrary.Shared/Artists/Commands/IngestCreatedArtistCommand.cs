using AutoMapper;
using FluentValidation;
using FolkLibrary.Artists.Events;
using FolkLibrary.Repositories;
using MediatR;

namespace FolkLibrary.Artists.Commands;

public sealed class IngestCreatedArtistCommand : IRequest
{
    public ArtistCreatedEvent Event { get; init; } = null!;

    public sealed class Validator : AbstractValidator<IngestCreatedArtistCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Event).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<IngestCreatedArtistCommand>
    {
        private readonly IMapper _mapper;
        private readonly IArtistDocumentRepository _artistDocumentRepository;

        public Handler(IMapper mapper, IArtistDocumentRepository artistDocumentRepository)
        {
            _mapper = mapper;
            _artistDocumentRepository = artistDocumentRepository;
        }

        public async Task<Unit> Handle(IngestCreatedArtistCommand request, CancellationToken cancellationToken)
        {
            var artist = _mapper.Map<ArtistDocument>(request.Event.Data);
            await _artistDocumentRepository.AddAsync(artist, cancellationToken);
            return Unit.Value;
        }
    }
}
