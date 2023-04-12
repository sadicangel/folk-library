using FluentValidation;
using FolkLibrary.Repositories;
using MediatR;
using OneOf.Types;

namespace FolkLibrary.Artists.Queries;

public sealed class GetArtistByIdQuery : IRequest<Response<ArtistDto>>
{
    public required string ArtistId { get; init; }

    public sealed class Validator : AbstractValidator<GetArtistByIdQuery>
    {
        public Validator() => RuleFor(e => e.ArtistId).NotEmpty();
    }

    public sealed class Handler : IRequestHandler<GetArtistByIdQuery, Response<ArtistDto>>
    {
        private readonly IArtistViewRepository _artistViewRepository;

        public Handler(IArtistViewRepository artistViewRepository)
        {
            _artistViewRepository = artistViewRepository;
        }

        public async Task<Response<ArtistDto>> Handle(GetArtistByIdQuery request, CancellationToken cancellationToken)
        {
            var artist = await _artistViewRepository.GetByIdAsync(request.ArtistId, cancellationToken);
            if (artist is null)
                return new NotFound();
            return artist;
        }
    }
}
