using Ardalis.Specification;
using AutoMapper;
using FluentValidation;
using FolkLibrary.Application.Interfaces;
using FolkLibrary.Artists.Events;
using FolkLibrary.Errors;
using FolkLibrary.Repositories;
using FolkLibrary.Services;
using MediatR;

namespace FolkLibrary.Artists.Commands;

public sealed class CreateArtistCommand : IRequest<Response<ArtistDto>>, IMapTo<Artist>
{
    public required string Name { get; init; }

    public required string ShortName { get; init; }

    public string? Description { get; init; }

    public int? Year { get; init; }

    public bool IsYearUncertain { get; init; }

    public required HashSet<string> Genres { get; init; }

    public required string Country { get; init; }

    public string? District { get; init; }

    public string? Municipality { get; init; }

    public string? Parish { get; init; }

    public bool IsAbroad { get; init; }

    public sealed class Validator : AbstractValidator<CreateArtistCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Name).NotEmpty();
            RuleFor(e => e.ShortName).NotEmpty();
            RuleFor(e => e.Description).MaximumLength(byte.MaxValue);
            RuleFor(e => e.Year).NotEmpty();
            RuleFor(e => e.Genres).NotEmpty();
            RuleFor(e => e.Country).NotEmpty().Length(exactLength: 2);
        }
    }

    public sealed class FindByNameSpecification : Specification<Artist>
    {
        public FindByNameSpecification(string name)
        {
            Query.Where(e => e.Name == name);
        }
    }

    public sealed class Handler : IRequestHandler<CreateArtistCommand, Response<ArtistDto>>
    {
        private readonly IPublisher _mediator;
        private readonly IMapper _mapper;
        private readonly IArtistRepository _artistRepository;
        private readonly IValidatorService<CreateArtistCommand> _validator;

        public Handler(IValidatorService<CreateArtistCommand> validator, IPublisher mediator, IMapper mapper, IArtistRepository artistRepository)
        {
            _validator = validator;
            _mediator = mediator;
            _mapper = mapper;
            _artistRepository = artistRepository;
        }

        public async Task<Response<ArtistDto>> Handle(CreateArtistCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            if (await _artistRepository.AnyAsync(new FindByNameSpecification(request.Name), cancellationToken))
                return new AlreadyExists();

            var artist = _mapper.Map<Artist>(request);

            await _artistRepository.AddAsync(artist, cancellationToken);
            await _mediator.Publish(new ArtistCreatedEvent { ArtistId = artist.Id }, cancellationToken);

            return _mapper.Map<ArtistDto>(artist);
        }
    }
}