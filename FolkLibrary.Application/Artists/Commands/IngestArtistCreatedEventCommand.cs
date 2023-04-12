using AutoMapper;
using FluentValidation;
using FolkLibrary.Artists.Events;
using FolkLibrary.Errors;
using FolkLibrary.Repositories;
using FolkLibrary.Services;
using MediatR;
using OneOf.Types;

namespace FolkLibrary.Artists.Commands;

public sealed class IngestArtistCreatedEventCommand : IRequest<Response<Success>>
{
    public required ArtistCreatedEvent Event { get; init; }

    public sealed class Validator : AbstractValidator<IngestArtistCreatedEventCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Event).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<IngestArtistCreatedEventCommand, Response<Success>>
    {
        private readonly IValidatorService<IngestArtistCreatedEventCommand> _validator;
        private readonly IMapper _mapper;
        private readonly IArtistRepository _artistRepository;
        private readonly IArtistViewRepository _artistViewRepository;

        public Handler(IValidatorService<IngestArtistCreatedEventCommand> validator, IMapper mapper, IArtistRepository artistRepository, IArtistViewRepository artistViewRepository)
        {
            _validator = validator;
            _mapper = mapper;
            _artistRepository = artistRepository;
            _artistViewRepository = artistViewRepository;
        }

        public async Task<Response<Success>> Handle(IngestArtistCreatedEventCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var artist = await _artistRepository.GetByIdAsync(request.Event.ArtistId, cancellationToken);
            if (artist is null)
                return new NotFound();

            await _artistViewRepository.AddAsync(_mapper.Map<ArtistDto>(artist), cancellationToken);
            return new Success();
        }
    }
}
