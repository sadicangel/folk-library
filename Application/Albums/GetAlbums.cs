using DotNext;
using FluentValidation;
using Marten;
using MediatR;

namespace FolkLibrary.Albums;
public sealed record class GetAlbumsResponse(int Results, IReadOnlyList<Album> Albums);

public sealed record class GetAlbums(string? Name, int? Year, int? BeforeYear, int? AfterYear, OrderBy? OrderBy) : IRequest<Result<GetAlbumsResponse>>;

public sealed class GetAlbumsValidator : AbstractValidator<GetAlbums>
{
    private static readonly List<string> PropertyNames = typeof(Album).GetProperties().Select(p => p.Name).ToList();

    public GetAlbumsValidator()
    {
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

public sealed class GetAlbumsHandler : IRequestHandler<GetAlbums, Result<GetAlbumsResponse>>
{
    private readonly IValidator<GetAlbums> _validator;
    private readonly IDocumentSession _documentSession;

    public GetAlbumsHandler(IValidator<GetAlbums> validator, IDocumentSession documentSession)
    {
        _validator = validator;
        _documentSession = documentSession;
    }

    public async Task<Result<GetAlbumsResponse>> Handle(GetAlbums request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new Result<GetAlbumsResponse>(new ValidationException(validationResult.Errors));

        var query = new QueryBuilder()
            .Where(request.Name, nameof(Album.Name), "ilike")
            .Where(request.Year, nameof(Album.Year), "=")
            .Where(request.AfterYear, nameof(Album.Year), ">=")
            .Where(request.BeforeYear, nameof(Album.Year), "<=")
            .OrderBy(request.OrderBy)
            .Build();
        var albums = await query.Match(
            some: q => _documentSession.QueryAsync<Album>(q.Where, cancellationToken, q.Params),
            none: () => _documentSession.Query<Album>().ToListAsync(cancellationToken));

        return new GetAlbumsResponse(albums.Count, albums);
    }
}