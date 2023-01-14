using AutoMapper;
using FluentValidation;
using FolkLibrary.Albums.Events;
using FolkLibrary.Interfaces;
using FolkLibrary.Repositories;
using FolkLibrary.Tracks;
using MediatR;

namespace FolkLibrary.Albums.Commands;

public sealed class CreateAlbumCommand : IRequest<AlbumId>, IMapTo<Album>
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? Year { get; set; }

    public bool IsYearUncertain { get; set; }

    public HashSet<Genre> Genres { get; set; } = null!;

    public TimeSpan Duration { get; set; }

    public List<CreateTrackDto> Tracks { get; set; } = null!;

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

    public sealed class Handler : IRequestHandler<CreateAlbumCommand, AlbumId>
    {
        private readonly IPublisher _mediator;
        private readonly IMapper _mapper;
        private readonly IAlbumEntityRepository _albumEntityRepository;

        public Handler(IPublisher mediator, IMapper mapper, IAlbumEntityRepository albumEntityRepository)
        {
            _mediator = mediator;
            _mapper = mapper;
            _albumEntityRepository = albumEntityRepository;
        }

        public async Task<AlbumId> Handle(CreateAlbumCommand request, CancellationToken cancellationToken)
        {
            var album = _mapper.Map<Album>(request);
            album.TrackCount = album.Tracks.Count;
            album.Duration = album.Tracks.Aggregate(TimeSpan.Zero, (p, c) => p + c.Duration);
            album.IsIncomplete = album.TrackCount != album.Tracks.Max(t => t.Number);
            await _albumEntityRepository.AddAsync(album, cancellationToken);
            await _mediator.Publish(new AlbumCreatedEvent
            {
                Data = _mapper.Map<AlbumCreatedEventData>(album)
            }, cancellationToken);
            return album.Id;
        }
    }
}
public sealed class CreateTrackDto : IMapTo<Track>
{
    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Description { get; set; }

    public int? Year { get; set; }

    public bool IsYearUncertain { get; set; }

    public HashSet<Genre> Genres { get; set; } = new();

    public int Number { get; set; }

    public TimeSpan Duration { get; set; }
}
