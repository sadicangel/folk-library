using Ardalis.Specification;
using AutoMapper;
using FluentValidation;
using FolkLibrary.Exceptions;
using FolkLibrary.Interfaces;
using FolkLibrary.Models;
using MediatR;

namespace FolkLibrary.Commands.Artists;

public sealed class CreateArtistCommand : IRequest<ArtistId>, IMapTo<Artist>
{
    public string Name { get; set; } = null!;

    public string ShortName { get; set; } = null!;

    public string? Description { get; set; }

    public int? Year { get; set; }

    public bool IsYearUncertain { get; set; }

    public HashSet<Genre> Genres { get; set; } = new();

    public string Country { get; set; } = null!;

    public string? District { get; set; }

    public string? Municipality { get; set; }

    public string? Parish { get; set; }

    public bool IsAbroad { get; set; }

    public sealed class Validator : AbstractValidator<CreateArtistCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Name).NotEmpty();
            RuleFor(e => e.ShortName).NotEmpty();
            RuleFor(e => e.Description).MaximumLength(Byte.MaxValue);
            RuleFor(e => e.Year).NotEmpty();
            RuleFor(e => e.Genres).NotEmpty();
            RuleFor(e => e.Country).NotEmpty().Length(exactLength: 3);
        }
    }

    public sealed class FindByNameSpecification : Specification<Artist>
    {
        public FindByNameSpecification(string name)
        {
            Query.Where(e => e.Name == name);
        }
    }

    public sealed class Handler : IRequestHandler<CreateArtistCommand, ArtistId>
    {
        private readonly IMapper _mapper;
        private readonly IPostgresRepository<Artist> _artistRepository;

        public Handler(IMapper mapper, IPostgresRepository<Artist> artistRepository)
        {
            _mapper = mapper;
            _artistRepository = artistRepository;
        }

        public async Task<ArtistId> Handle(CreateArtistCommand request, CancellationToken cancellationToken)
        {
            if (await _artistRepository.AnyAsync(new FindByNameSpecification(request.Name), cancellationToken))
                throw new ConflictException($"An artist with name '{request.Name}' already exists");
            var artist = _mapper.Map<Artist>(request);
            await _artistRepository.AddAsync(artist, cancellationToken);
            return artist.Id;
        }
    }
}
