using AutoMapper;
using FluentValidation;
using FolkLibrary.Artists.Events;
using FolkLibrary.Repositories;
using MediatR;

namespace FolkLibrary.Artists.Commands;
public sealed class IngestArtistDeletedCommand : IRequest
{
    public ArtistDeletedEvent Event { get; init; } = null!;

    public sealed class Validator : AbstractValidator<IngestArtistDeletedCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Event).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<IngestArtistDeletedCommand>
    {
        private readonly IMapper _mapper;
        private readonly IArtistDocumentRepository _artistDocumentRepository;

        public Handler(IMapper mapper, IArtistDocumentRepository artistDocumentRepository)
        {
            _mapper = mapper;
            _artistDocumentRepository = artistDocumentRepository;
        }

        public async Task<Unit> Handle(IngestArtistDeletedCommand request, CancellationToken cancellationToken)
        {
            await _artistDocumentRepository.DeleteAsync(_mapper.Map<ArtistDocument>(request.Event.Data), cancellationToken);
            return Unit.Value;
        }
    }
}
