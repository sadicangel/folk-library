using Ardalis.Specification;
using FastEndpoints;
using FluentValidation;
using FolkLibrary.Application.Interfaces;
using FolkLibrary.Artists;
using FolkLibrary.Artists.Events;
using FolkLibrary.Repositories;
using System.Net;
using System.Net.Mime;
using IMapper = AutoMapper.IMapper;

namespace FolkLibrary.Endpoints;

public sealed class CreateArtistRequest : IMapTo<Artist>
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

    public sealed class Validator : Validator<CreateArtistRequest>
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
}

public sealed class CreateArtistEndpoint : Endpoint<CreateArtistRequest, ArtistDto>
{
    private readonly IMapper _mapper;
    private readonly IArtistRepository _artistRepository;

    public CreateArtistEndpoint(IMapper mapper, IArtistRepository artistRepository)
    {
        _mapper = mapper;
        _artistRepository = artistRepository;
    }

    public override void Configure()
    {
        Post("/api/artist");
    }

    public override async Task HandleAsync(CreateArtistRequest request, CancellationToken cancellationToken)
    {
        if (await _artistRepository.AnyAsync(new FindByNameSpecification(request.Name), cancellationToken))
        {
            await SendStringAsync($"'{request.Name}' already exists", (int)HttpStatusCode.Conflict, MediaTypeNames.Text.Plain, cancellationToken);
            return;
        }
        var artist = _mapper.Map<Artist>(request);

        await _artistRepository.AddAsync(artist, cancellationToken);
        await PublishAsync(new ArtistCreatedEvent { ArtistId = artist.Id }, Mode.WaitForNone, cancellationToken);

        await SendCreatedAtAsync<GetArtistByIdEndpoint>(new { artistId = artist.Id }, _mapper.Map<ArtistDto>(artist), cancellation: cancellationToken);
    }

    private sealed class FindByNameSpecification : Specification<Artist>
    {
        public FindByNameSpecification(string name)
        {
            Query.Where(e => e.Name == name);
        }
    }
}