using FastEndpoints;
using FluentValidation;
using FolkLibrary.Albums;
using FolkLibrary.Application.Interfaces;
using FolkLibrary.Repositories;
using FolkLibrary.Tracks;
using IMapper = AutoMapper.IMapper;

namespace FolkLibrary.Endpoints;

public sealed class CreateAlbumRequest : IMapTo<Album>
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public int? Year { get; init; }
    public bool IsYearUncertain { get; init; }
    public required HashSet<string> Genres { get; init; }
    public required TimeSpan Duration { get; init; }
    public required List<CreateTrackDto> Tracks { get; init; }

    public sealed class Validator : Validator<CreateAlbumRequest>
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
}

public sealed class CreateAlbumEndpoint : Endpoint<CreateAlbumRequest, AlbumDto>
{
    private readonly IMapper _mapper;
    private readonly IAlbumRepository _albumRepository;

    public CreateAlbumEndpoint(IMapper mapper, IAlbumRepository albumRepository)
    {
        _mapper = mapper;
        _albumRepository = albumRepository;
    }

    public override void Configure()
    {
        Post("/api/album");
    }

    public override async Task HandleAsync(CreateAlbumRequest request, CancellationToken cancellationToken)
    {
        var album = _mapper.Map<Album>(request);
        album.Duration = album.Tracks.Aggregate(TimeSpan.Zero, (p, c) => p + c.Duration);
        album.IsIncomplete = album.TrackCount != album.Tracks.Max(t => t.Number);
        await _albumRepository.AddAsync(album, cancellationToken);

        await SendOkAsync(_mapper.Map<AlbumDto>(album), cancellationToken);
    }
}
