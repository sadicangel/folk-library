using Ardalis.Specification;
using FluentValidation;
using FolkLibrary.Dtos;
using FolkLibrary.Interfaces;
using FolkLibrary.Models;
using MediatR;

namespace FolkLibrary.Queries.Artists;

public sealed class GetAllArtistsQueryParams : IQueryParams<GetAllArtistsQueryParams>
{
    public string? Country { get; init; }
    public string? District { get; init; }
    public string? Municipality { get; init; }
    public string? Parish { get; init; }
    public int? AfterYear { get; init; }
    public int? BeforeYear { get; init; }
}

public sealed class GetAllArtistsQuery : IRequest<Page<ArtistDto>>
{
    public GetAllArtistsQueryParams QueryParams { get; init; } = null!;
    public string? ContinuationToken { get; init; }

    public sealed class Validator : AbstractValidator<GetAllArtistsQuery>
    {
        public Validator()
        {
            RuleFor(e => e.QueryParams).ChildRules(v =>
            {
                v.When(e => e.Country is not null, () => v.RuleFor(e => e.Country).Length(3));
                v.When(e => e.AfterYear is not null, () => v.RuleFor(e => e.AfterYear).GreaterThanOrEqualTo(1900));
                v.When(e => e.BeforeYear is not null, () => v.RuleFor(e => e.BeforeYear).GreaterThanOrEqualTo(1900));
            });
        }
    }

    public sealed class Specification : Specification<ArtistDto>
    {
        public Specification(GetAllArtistsQueryParams queryParams)
        {
            Query.OrderBy(a => a.Year).ThenBy(a => a.ShortName);
            if (queryParams.Country is not null)
                Query.Where(a => a.Country == queryParams.Country);
            if (queryParams.District is not null)
                Query.Where(a => a.District == queryParams.District);
            if (queryParams.Municipality is not null)
                Query.Where(a => a.Municipality == queryParams.Municipality);
            if (queryParams.Parish is not null)
                Query.Where(a => a.Parish == queryParams.Parish);
            if (queryParams.AfterYear is not null)
                Query.Where(a => a.Year >= queryParams.AfterYear);
            if (queryParams.BeforeYear is not null)
                Query.Where(a => a.Year <= queryParams.BeforeYear);
        }
    }

    public sealed class Handler : IRequestHandler<GetAllArtistsQuery, Page<ArtistDto>>
    {
        private readonly IMongoRepository<ArtistDto> _artistRepository;

        public Handler(IMongoRepository<ArtistDto> artistRepository)
        {
            _artistRepository = artistRepository;
        }

        public async Task<Page<ArtistDto>> Handle(GetAllArtistsQuery request, CancellationToken cancellationToken)
        {
            var specification = new Specification(request.QueryParams);
            var artists = await _artistRepository.ListPagedAsync(specification, request.ContinuationToken, cancellationToken);
            return artists;
        }
    }
}
