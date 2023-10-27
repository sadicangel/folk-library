using DotNext;
using FluentValidation;
using Marten;
using MediatR;

namespace FolkLibrary.Artists;

public sealed record class GetArtistsResponse(int Results, IReadOnlyList<Artist> Artists);

public sealed record class GetArtists(
    string? Name = null,
    string? CountryCode = null,
    string? CountryName = null,
    string? District = null,
    string? Municipality = null,
    string? Parish = null,
    int? Year = null,
    int? AfterYear = null,
    int? BeforeYear = null,
    OrderBy? OrderBy = null
) : IRequest<Result<GetArtistsResponse>>;

public sealed class GetArtistsValidator : AbstractValidator<GetArtists>
{
    private static readonly List<string> PropertyNames = typeof(Artist).GetProperties().Select(p => p.Name).ToList();

    public GetArtistsValidator()
    {
        RuleFor(r => r.CountryCode).Length(2).When(r => !String.IsNullOrEmpty(r.CountryCode));
        RuleFor(r => r.Year).InclusiveBetween(1900, 2100).When(r => r.Year is not null);
        RuleFor(r => r.AfterYear).InclusiveBetween(1900, 2100).When(r => r.AfterYear is not null);
        RuleFor(r => r.AfterYear).GreaterThanOrEqualTo(r => r.BeforeYear).When(r => r.AfterYear is not null && r.BeforeYear is not null);
        RuleFor(r => r.BeforeYear).InclusiveBetween(1900, 2100).When(r => r.BeforeYear is not null);
        RuleFor(r => r.BeforeYear).LessThanOrEqualTo(r => r.AfterYear).When(r => r.AfterYear is not null && r.BeforeYear is not null);
        RuleFor(r => r.OrderBy).ChildRules(v => v.RuleFor(r => r!.Value).ChildRules(w =>
        {
            w.RuleFor(r => r.PropertyName).Must(PropertyNames.Contains).WithMessage($"Must be one of {String.Join(", ", PropertyNames)}");
            w.RuleFor(r => r.Direction).IsInEnum();
        }).When(r => r.HasValue)).When(r => r.OrderBy is not null);
    }
}

public sealed class GetArtistsHandler : IRequestHandler<GetArtists, Result<GetArtistsResponse>>
{
    private readonly IValidator<GetArtists> _validator;
    private readonly IDocumentSession _documentSession;

    public GetArtistsHandler(IValidator<GetArtists> validator, IDocumentSession documentSession)
    {
        _validator = validator;
        _documentSession = documentSession;
    }

    public async Task<Result<GetArtistsResponse>> Handle(GetArtists request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new Result<GetArtistsResponse>(new ValidationException(validationResult.Errors));

        var query = new QueryBuilder()
            .Where(request.Name, nameof(Artist.Name), "ilike")
            .Where(request.CountryCode, nameof(Artist.Location), nameof(Location.CountryCode), "=")
            .Where(request.CountryName, nameof(Artist.Location), nameof(Location.CountryName), "=")
            .Where(request.District, nameof(Artist.Location), nameof(Location.District), "=")
            .Where(request.Municipality, nameof(Artist.Location), nameof(Location.Municipality), "=")
            .Where(request.Parish, nameof(Artist.Location), nameof(Location.Parish), "=")
            .Where(request.Year, nameof(Artist.Year), "=")
            .Where(request.AfterYear, nameof(Artist.Year), ">=")
            .Where(request.BeforeYear, nameof(Artist.Year), "<=")
            .OrderBy(request.OrderBy)
            .Build();
        var artists = await query.Match(
            some: q => _documentSession.QueryAsync<Artist>(q.Where, cancellationToken, q.Params),
            none: () => _documentSession.Query<Artist>().ToListAsync(cancellationToken));

        return new GetArtistsResponse(artists.Count, artists);
    }
}