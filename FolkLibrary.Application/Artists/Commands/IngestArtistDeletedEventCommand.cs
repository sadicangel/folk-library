using FluentValidation;
using FolkLibrary.Artists.Events;
using FolkLibrary.Errors;
using FolkLibrary.Repositories;
using FolkLibrary.Services;
using MediatR;
using OneOf.Types;

namespace FolkLibrary.Artists.Commands;
public sealed class IngestArtistDeletedEventCommand : IRequest<Response<Success>>
{
    public required ArtistDeletedEvent Event { get; init; }

    public sealed class Validator : AbstractValidator<IngestArtistDeletedEventCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Event).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<IngestArtistDeletedEventCommand, Response<Success>>
    {
        private readonly IValidatorService<IngestArtistDeletedEventCommand> _validator;
        private readonly IArtistViewRepository _artistViewRepository;

        public Handler(IValidatorService<IngestArtistDeletedEventCommand> validator, IArtistViewRepository artistViewRepository)
        {
            _validator = validator;
            _artistViewRepository = artistViewRepository;
        }

        public async Task<Response<Success>> Handle(IngestArtistDeletedEventCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var artist = await _artistViewRepository.GetByIdAsync(request.Event.Id, cancellationToken);
            if (artist is null)
                return new NotFound();

            await _artistViewRepository.DeleteAsync(artist, cancellationToken);
            return new Success();
        }
    }
}
