using Ardalis.Specification;
using FastEndpoints;
using FluentValidation;
using FolkLibrary.Artists;
using FolkLibrary.Repositories;

namespace FolkLibrary.Endpoints;

public sealed class GetArtistsRequest
{
    [FromQueryParams]
    public ArtistFilterDto? Filter { get; init; }

    [FromHeader(IsRequired = false)]
    public int? PageIndex { get; init; }

    [FromHeader(IsRequired = false)]
    public int? PageSize { get; init; }

    public sealed class Validator : Validator<GetArtistsRequest>
    {
        public Validator()
        {
            When(e => e.Filter is not null, () => RuleFor(e => e.Filter).ChildRules(v =>
            {
                v.When(e => e!.Country is not null, () => v.RuleFor(e => e!.Country).Length(2));
                v.When(e => e!.AfterYear is not null, () => v.RuleFor(e => e!.AfterYear).GreaterThanOrEqualTo(1900));
                v.When(e => e!.BeforeYear is not null, () => v.RuleFor(e => e!.BeforeYear).GreaterThanOrEqualTo(1900));
            }));
            When(e => e.PageIndex.HasValue, () => RuleFor(e => e.PageIndex).GreaterThan(0));
            When(e => e.PageSize.HasValue, () => RuleFor(e => e.PageSize).GreaterThan(0));
        }
    }
}

public sealed class GetArtistsEndpoint : Endpoint<GetArtistsRequest, Page<ArtistDto>>
{
    private readonly IArtistViewRepository _artistViewRepository;

    public GetArtistsEndpoint(IArtistViewRepository artistViewRepository)
    {
        _artistViewRepository = artistViewRepository;
    }

    public override void Configure()
    {
        Get("/api/artist");
    }
    private sealed class Specification : Specification<ArtistDto>
    {
        public static ISpecification<ArtistDto> SortByYearThenShortName { get; } = new Specification(default);

        public Specification(ArtistFilterDto? filter)
        {
            Query.OrderBy(a => a.Year).ThenBy(a => a.ShortName);
            if (filter is not null)
            {
                if (filter.Country is not null)
                    Query.Where(a => a.Country == filter.Country);
                if (filter.District is not null)
                    Query.Where(a => a.District == filter.District);
                if (filter.Municipality is not null)
                    Query.Where(a => a.Municipality == filter.Municipality);
                if (filter.Parish is not null)
                    Query.Where(a => a.Parish == filter.Parish);
                if (filter.AfterYear is not null)
                    Query.Where(a => a.Year >= filter.AfterYear);
                if (filter.BeforeYear is not null)
                    Query.Where(a => a.Year <= filter.BeforeYear);
            }
        }
    }

    public override async Task HandleAsync(GetArtistsRequest request, CancellationToken cancellationToken)
    {
        var specification = request.Filter is not null ? new Specification(request.Filter) : Specification.SortByYearThenShortName;
        var artists = await _artistViewRepository.ListAsync(specification, request.PageIndex ?? 1, request.PageSize ?? 20, cancellationToken);
        await SendOkAsync(artists, cancellationToken);
    }
}