using Ardalis.Specification;
using FluentValidation;
using FolkLibrary.Repositories;
using MediatR;

namespace FolkLibrary.Artists.Queries;

public sealed class GetArtistsQuery : IRequest<Response<Page<ArtistDto>>>
{
    public ArtistFilterDto? Filter { get; init; }
    public string? ContinuationToken { get; init; }
    public int? PageSize { get; init; }

    public sealed class Validator : AbstractValidator<GetArtistsQuery>
    {
        public Validator()
        {
            When(e => e.Filter is not null, () => RuleFor(e => e.Filter).ChildRules(v =>
            {
                v.When(e => e!.Country is not null, () => v.RuleFor(e => e!.Country).Length(2));
                v.When(e => e!.AfterYear is not null, () => v.RuleFor(e => e!.AfterYear).GreaterThanOrEqualTo(1900));
                v.When(e => e!.BeforeYear is not null, () => v.RuleFor(e => e!.BeforeYear).GreaterThanOrEqualTo(1900));
            }));
        }
    }

    public sealed class Specification : Specification<ArtistDto>
    {
        public Specification(ArtistFilterDto? filter)
        {
            if (filter is not null)
            {
                Query.OrderBy(a => a.Year).ThenBy(a => a.ShortName);
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

    public sealed class Handler : IRequestHandler<GetArtistsQuery, Response<Page<ArtistDto>>>
    {
        private readonly IArtistViewRepository _artistViewRepository;

        public Handler(IArtistViewRepository artistViewRepository)
        {
            _artistViewRepository = artistViewRepository;
        }

        public async Task<Response<Page<ArtistDto>>> Handle(GetArtistsQuery request, CancellationToken cancellationToken)
        {
            var specification = new Specification(request.Filter);
            var artists = await _artistViewRepository.ListAsync(specification, request.ContinuationToken, request.PageSize ?? 20, cancellationToken);
            return artists;
        }
    }
}
