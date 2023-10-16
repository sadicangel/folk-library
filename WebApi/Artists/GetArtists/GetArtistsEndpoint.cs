using Ardalis.Specification;
using FastEndpoints;
using FluentValidation;
using FolkLibrary.Repositories;

namespace FolkLibrary.Artists.GetArtists;

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