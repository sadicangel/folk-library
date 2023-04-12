using AutoMapper;
using FluentValidation;
using FolkLibrary.Application.Interfaces;
using FolkLibrary.Errors;
using FolkLibrary.Repositories;
using FolkLibrary.Services;
using FolkLibrary.Tracks;
using MediatR;

namespace FolkLibrary.Albums.Commands;

public sealed class CreateAlbumCommand : IRequest<Response<AlbumDto>>, IMapTo<Album>
{
    public required string Name { get; init; }

    public string? Description { get; init; }

    public int? Year { get; init; }

    public bool IsYearUncertain { get; init; }

    public required HashSet<string> Genres { get; init; }

    public required TimeSpan Duration { get; init; }

    public required List<CreateTrackDto> Tracks { get; init; }

    public sealed class Validator : AbstractValidator<CreateAlbumCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Name).NotEmpty();
            RuleFor(e => e.Genres).NotEmpty();
            RuleFor(e => e.Tracks).NotEmpty();
            RuleForEach(e => e.Tracks).ChildRules(v =>
            {
                v.RuleFor(e => e.Name).NotEmpty();
                v.RuleFor(e => e.Genres).NotEmpty();
                v.RuleFor(e => e.Number).GreaterThan(0);
                v.RuleFor(e => e.Duration).GreaterThan(TimeSpan.Zero);
            });
        }
    }

    public sealed class Handler : IRequestHandler<CreateAlbumCommand, Response<AlbumDto>>
    {
        private readonly IValidatorService<CreateAlbumCommand> _validator;
        private readonly IMapper _mapper;
        private readonly IAlbumRepository _albumRepository;

        public Handler(IValidatorService<CreateAlbumCommand> validator, IMapper mapper, IAlbumRepository albumRepository)
        {
            _validator = validator;
            _mapper = mapper;
            _albumRepository = albumRepository;
        }

        public async Task<Response<AlbumDto>> Handle(CreateAlbumCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var album = _mapper.Map<Album>(request);
            album.TrackCount = album.Tracks.Count;
            album.Duration = album.Tracks.Aggregate(TimeSpan.Zero, (p, c) => p + c.Duration);
            album.IsIncomplete = album.TrackCount != album.Tracks.Max(t => t.Number);
            await _albumRepository.AddAsync(album, cancellationToken);

            return _mapper.Map<AlbumDto>(album);
        }
    }
}