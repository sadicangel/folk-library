using FluentValidation;
using FolkLibrary.Errors;
using FolkLibrary.Repositories;
using FolkLibrary.Services;
using MediatR;
using OneOf.Types;

namespace FolkLibrary.Artists.Commands;

public sealed class DeleteArtistCommand : IRequest<Response<Success>>
{
    public required string ArtistId { get; init; }

    public sealed class Validator : AbstractValidator<DeleteArtistCommand>
    {
        public Validator()
        {
            RuleFor(e => e.ArtistId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<DeleteArtistCommand, Response<Success>>
    {
        private readonly IValidatorService<DeleteArtistCommand> _validator;
        private readonly IArtistRepository _artistRepository;

        public Handler(IValidatorService<DeleteArtistCommand> validator, IArtistRepository artistRepository)
        {
            _validator = validator;
            _artistRepository = artistRepository;
        }

        public async Task<Response<Success>> Handle(DeleteArtistCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var artist = await _artistRepository.GetByIdAsync(request.ArtistId, cancellationToken);
            if (artist is null)
                return new NotFound();

            await _artistRepository.DeleteAsync(artist, cancellationToken);

            return new Success();
        }
    }
}