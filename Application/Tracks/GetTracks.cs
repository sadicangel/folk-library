using DotNext;
using FluentValidation;
using Marten;
using MediatR;

namespace FolkLibrary.Tracks;

public sealed record class GetTracksResponse(int Results, IReadOnlyList<Track> Tracks);

public sealed record class GetTracks(
    string? Name = null,
    int? Year = null,
    int? AfterYear = null,
    int? BeforeYear = null,
    TimeSpan? AboveDuration = null,
    TimeSpan? BelowDuration = null,
    OrderBy? OrderBy = null
) : IRequest<Result<GetTracksResponse>>;

public sealed class GetTracksValidator : AbstractValidator<GetTracks>
{
    private static readonly List<string> PropertyNames = typeof(Track).GetProperties().Select(p => p.Name).ToList();

    public GetTracksValidator()
    {
        RuleFor(r => r.Year).InclusiveBetween(1900, 2100).When(r => r.Year is not null);
        RuleFor(r => r.AfterYear).InclusiveBetween(1900, 2100).When(r => r.AfterYear is not null);
        RuleFor(r => r.AfterYear).GreaterThanOrEqualTo(r => r.BeforeYear).When(r => r.AfterYear is not null && r.BeforeYear is not null);
        RuleFor(r => r.BeforeYear).InclusiveBetween(1900, 2100).When(r => r.BeforeYear is not null);
        RuleFor(r => r.BeforeYear).LessThanOrEqualTo(r => r.AfterYear).When(r => r.AfterYear is not null && r.BeforeYear is not null);
        RuleFor(r => r.AboveDuration).GreaterThan(TimeSpan.Zero).When(r => r.AboveDuration is not null);
        RuleFor(r => r.AboveDuration).GreaterThanOrEqualTo(r => r.BelowDuration).When(r => r.AboveDuration is not null && r.BelowDuration is not null);
        RuleFor(r => r.BelowDuration).GreaterThan(TimeSpan.Zero).When(r => r.BelowDuration is not null);
        RuleFor(r => r.BelowDuration).LessThanOrEqualTo(r => r.BelowDuration).When(r => r.AboveDuration is not null && r.BelowDuration is not null);
        RuleFor(r => r.OrderBy).ChildRules(v => v.RuleFor(r => r!.Value).ChildRules(w =>
        {
            w.RuleFor(r => r.PropertyName).Must(PropertyNames.Contains).WithMessage($"Must be one of {String.Join(", ", PropertyNames)}");
            w.RuleFor(r => r.Direction).IsInEnum();
        }).When(r => r.HasValue)).When(r => r.OrderBy is not null);
    }
}

public sealed class GetTracksHandler : IRequestHandler<GetTracks, Result<GetTracksResponse>>
{
    private readonly IValidator<GetTracks> _validator;
    private readonly IDocumentSession _documentSession;

    public GetTracksHandler(IValidator<GetTracks> validator, IDocumentSession documentSession)
    {
        _validator = validator;
        _documentSession = documentSession;
    }

    public async Task<Result<GetTracksResponse>> Handle(GetTracks request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new Result<GetTracksResponse>(new ValidationException(validationResult.Errors));

        var query = new QueryBuilder()
            .Where(request.Name, nameof(Track.Name), "ilike")
            .Where(request.Year, nameof(Track.Year), "=")
            .Where(request.AfterYear, nameof(Track.Year), ">=")
            .Where(request.BeforeYear, nameof(Track.Year), "<=")
            .Where(request.AboveDuration, nameof(Track.Duration), " >= ")
            .Where(request.BelowDuration, nameof(Track.Duration), " <= ")
            .OrderBy(request.OrderBy)
            .Build();
        var artists = await query.Match(
            some: q => _documentSession.QueryAsync<Track>(q.Where, cancellationToken, q.Params),
            none: () => _documentSession.Query<Track>().ToListAsync(cancellationToken));

        return new GetTracksResponse(artists.Count, artists);
    }
}