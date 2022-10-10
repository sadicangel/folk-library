using Ardalis.Specification;
using FluentValidation;
using FolkLibrary.Dtos;
using FolkLibrary.Interfaces;
using FolkLibrary.Models;
using MediatR;

namespace FolkLibrary.Queries.Artists;

public sealed class GetAllArtistsQuery : IRequest<Page<ArtistDto>>
{
    public string? Country { get; init; }
    public string? District { get; init; }
    public string? Municipality { get; init; }
    public string? Parish { get; init; }
    public int? AfterYear { get; init; }
    public int? BeforeYear { get; init; }
    public string? ContinuationToken { get; init; }

    public sealed class Validator : AbstractValidator<GetAllArtistsQuery>
    {
        public Validator()
        {
            When(e => e.Country is not null, () => RuleFor(e => e.Country).Length(3));
            When(e => e.AfterYear is not null, () => RuleFor(e => e.AfterYear).GreaterThanOrEqualTo(1900));
            When(e => e.BeforeYear is not null, () => RuleFor(e => e.BeforeYear).GreaterThanOrEqualTo(1900));
        }
    }

    public sealed class Specification : Specification<ArtistDto>
    {
        public Specification(string? country, string? district, string? municipality, string? parish, int? afterYear, int? beforeYear)
        {
            Query.OrderBy(a => a.Year).ThenBy(a => a.ShortName);
            if (country is not null)
                Query.Where(a => a.Country == country);
            if (district is not null)
                Query.Where(a => a.District == district);
            if (municipality is not null)
                Query.Where(a => a.Municipality == municipality);
            if (parish is not null)
                Query.Where(a => a.Parish == parish);
            if (afterYear is not null)
                Query.Where(a => a.Year >= afterYear);
            if (beforeYear is not null)
                Query.Where(a => a.Year <= beforeYear);
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
            var specification = new Specification(request.Country, request.District, request.Municipality, request.Parish, request.AfterYear, request.BeforeYear);
            var artists = await _artistRepository.ListPagedAsync(specification, request.ContinuationToken, cancellationToken);
            return artists;
        }
    }
}
