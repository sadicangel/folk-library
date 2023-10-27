using DotNext;
using FluentValidation;
using Marten;
using MediatR;
using System.Text;

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
    Sort? Sort = null
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
        RuleFor(r => r.Sort).ChildRules(v => v.RuleFor(r => r!.Value).ChildRules(w =>
        {
            w.RuleFor(r => r.PropertyName).Must(PropertyNames.Contains).WithMessage($"Must be one of {String.Join(", ", PropertyNames)}");
            w.RuleFor(r => r.SortDirection).IsInEnum();
        }).When(r => r.HasValue)).When(r => r.Sort is not null);
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
            .OrderBy(request.Sort)
            .Build();
        var artists = await query.Match(
            some: q => _documentSession.QueryAsync<Artist>(q.Where, cancellationToken, q.Params),
            none: () => _documentSession.Query<Artist>().ToListAsync(cancellationToken));

        return new GetArtistsResponse(artists.Count, artists);
    }
}

file struct QueryBuilder
{
    private List<string>? _wheres;
    private List<object>? _params;
    private List<string>? _sorts;

    private List<string> Wheres { get => _wheres ??= new List<string>(); }

    private List<object> Params { get => _params ??= new List<object>(); }

    private List<string> Sorts { get => _sorts ??= new List<string>(); }

    public QueryBuilder Where(string? value, string name, string condition)
    {
        if (!String.IsNullOrEmpty(value))
        {
            Wheres.Add($"data ->> '{name}' {condition} ?");
            if ("ilike".Equals(condition) || "like".Equals(condition))
                value = $"%{value}%";
            Params.Add(value);
        }
        return this;
    }

    public QueryBuilder Where(string? value, string name1, string name2, string condition)
    {
        if (!String.IsNullOrEmpty(value))
        {
            Wheres.Add($"data -> '{name1}' ->> '{name2}' {condition} ?");
            if ("ilike".Equals(condition) || "like".Equals(condition))
                value = $"%{value}%";
            Params.Add(value);
        }
        return this;
    }

    public QueryBuilder Where(int? value, string name, string condition)
    {
        if (value is not null)
        {
            Wheres.Add($"(data -> '{name}')::int {condition} ?");
            Params.Add(value);
        }
        return this;
    }

    public QueryBuilder OrderBy(Sort? sort)
    {
        if (sort is not null)
        {
            Sorts.Add($" order by data -> '{sort.Value.PropertyName}' {sort.Value.SortDirection}");
        }
        return this;
    }

    public readonly Optional<(string Where, object[] Params)> Build()
    {
        var builder = default(StringBuilder);
        if (_wheres is not null)
        {
            builder ??= new StringBuilder();
            builder.Append($" where {_wheres[0]}");
            foreach (var where in _wheres.Skip(1))
                builder.Append($" and {where}");
        }
        if (_sorts is not null)
        {
            builder ??= new StringBuilder();
            foreach (var sort in _sorts)
                builder.Append(sort);
        }

        return builder is { Length: > 0 }
            ? (builder.ToString(), _params?.ToArray() ?? Array.Empty<string>())
            : Optional<(string, object[])>.None;

    }
}