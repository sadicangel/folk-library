using DotNext;
using FluentValidation;
using Marten;
using MediatR;
using System.Text;

namespace FolkLibrary;

public sealed record class GetArtistsResponse(IReadOnlyList<Artist> Artists);

public sealed record class GetArtistsCommand(
    string? CountryCode = null,
    string? CountryName = null,
    string? District = null,
    string? Municipality = null,
    string? Parish = null,
    int? AfterYear = null,
    int? BeforeYear = null) : IRequest<Result<GetArtistsResponse>>;

public sealed class GetArtistsCommandValidator : AbstractValidator<GetArtistsCommand>
{
    public GetArtistsCommandValidator()
    {
        RuleFor(r => r.CountryCode).Length(2).When(r => r.CountryCode is not null);
        RuleFor(r => r.AfterYear).InclusiveBetween(1900, 2100).When(r => r.AfterYear is not null);
        RuleFor(r => r.AfterYear).GreaterThanOrEqualTo(r => r.BeforeYear).When(r => r.AfterYear is not null && r.BeforeYear is not null);
        RuleFor(r => r.BeforeYear).InclusiveBetween(1900, 2100).When(r => r.BeforeYear is not null);
        RuleFor(r => r.BeforeYear).LessThanOrEqualTo(r => r.AfterYear).When(r => r.AfterYear is not null && r.BeforeYear is not null);
    }
}

public sealed class GetArtistsCommandHandler : IRequestHandler<GetArtistsCommand, Result<GetArtistsResponse>>
{
    private readonly IDocumentSession _documentSession;

    public GetArtistsCommandHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<Result<GetArtistsResponse>> Handle(GetArtistsCommand request, CancellationToken cancellationToken)
    {
        var query = GetParameterizedQuery(request);
        var artists = await query.Match(
            some: q => _documentSession.QueryAsync<Artist>(q.Where, cancellationToken, q.Params),
            none: () => _documentSession.Query<Artist>().ToListAsync(cancellationToken));

        return new GetArtistsResponse(artists);
    }

    private static Optional<(string Where, object[] Params)> GetParameterizedQuery(GetArtistsCommand request)
    {
        StringBuilder? _builder = null;
        List<object>? _params = null;
        StringBuilder Builder() => _builder ??= new StringBuilder();
        List<object> Params() => _params ??= new List<object>();
        string Keyword() => _params is { Count: > 0 } ? " and" : "where";

        if (request.CountryCode is not null)
        {
            Builder().Append($"{Keyword()} data -> '{nameof(Artist.Location)}' ->> '{nameof(Location.CountryCode)}' = ?");
            Params().Add(request.CountryCode);
        }
        if (request.CountryName is not null)
        {
            Builder().Append($"{Keyword()} data -> '{nameof(Artist.Location)}' ->> '{nameof(Location.CountryName)}' = ?");
            Params().Add(request.CountryName);
        }
        if (request.District is not null)
        {
            Builder().Append($"{Keyword()} data -> '{nameof(Artist.Location)}' ->> '{nameof(Location.District)}' = ?");
            Params().Add(request.District);
        }
        if (request.Municipality is not null)
        {
            Builder().Append($"{Keyword()} data -> '{nameof(Artist.Location)}' ->> '{nameof(Location.Municipality)}' = ?");
            Params().Add(request.Municipality);
        }
        if (request.Parish is not null)
        {
            Builder().Append($"{Keyword()} data -> '{nameof(Artist.Location)}' ->> '{nameof(Location.Parish)}' = ?");
            Params().Add(request.Parish);
        }
        if (request.AfterYear is not null)
        {
            Builder().Append($"{Keyword()} (data -> '{nameof(Artist.Year)}')::int >= ?");
            Params().Add(request.AfterYear.Value);
        }
        if (request.BeforeYear is not null)
        {
            Builder().Append($"{Keyword()} (data -> '{nameof(Artist.Year)}')::int <= ?");
            Params().Add(request.BeforeYear.Value);
        }

        return _params is not null && _builder is not null
            ? (_builder.ToString(), _params.ToArray())
            : Optional<(string, object[])>.None;
    }
}