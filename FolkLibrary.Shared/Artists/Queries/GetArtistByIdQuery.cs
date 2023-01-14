using FluentValidation;
using FolkLibrary.Exceptions;
using FolkLibrary.Repositories;
using MediatR;

namespace FolkLibrary.Artists.Queries;

public sealed class GetArtistByIdQuery : IRequest<ArtistDocument>
{
    public ArtistId ArtistId { get; init; }

    public sealed class Validator : AbstractValidator<GetArtistByIdQuery>
    {
        public Validator() => RuleFor(e => e.ArtistId).NotEmpty();
    }

    public sealed class Handler : IRequestHandler<GetArtistByIdQuery, ArtistDocument>
    {
        private readonly IArtistDocumentRepository _artistDocumentRepository;

        public Handler(IArtistDocumentRepository artistDocumentRepository)
        {
            _artistDocumentRepository = artistDocumentRepository;
        }

        public async Task<ArtistDocument> Handle(GetArtistByIdQuery request, CancellationToken cancellationToken)
        {
            var artist = await _artistDocumentRepository.GetByIdAsync(request.ArtistId, cancellationToken);
            if (artist is null)
                throw new NotFoundException($"Artist does not exist ({request.ArtistId})");
            return artist;
        }
    }
}
