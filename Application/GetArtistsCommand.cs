using DotNext;
using FluentValidation;
using Marten;
using MediatR;

namespace FolkLibrary;

public sealed record class GetArtistsResponse(List<Artist> Artists);

public sealed record class GetArtistsCommand(
    string? Country = null,
    string? District = null,
    string? Municipality = null,
    string? Parish = null,
    int? AfterYear = null,
    int? BeforeYear = null
) : IRequest<Result<GetArtistsResponse>>;

public sealed class GetArtistsCommandValidator : AbstractValidator<GetArtistsCommand>
{
    public GetArtistsCommandValidator()
    {
        RuleFor(r => r.Country).Length(2).When(r => r.Country is not null);
        RuleFor(r => r.AfterYear).InclusiveBetween(1900, 2100).When(r => r.AfterYear is not null);
        RuleFor(r => r.BeforeYear).InclusiveBetween(1900, 2100).When(r => r.BeforeYear is not null);
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
        var streamIds = _documentSession.Events.QueryAllRawEvents().Select(e => e.StreamId).Distinct().ToList();

        var list = new List<Artist>();

        foreach (var streamId in streamIds)
        {
            var artist = await _documentSession.Events.AggregateStreamAsync<Artist>(streamId, token: cancellationToken);
            if (artist is null)
                return new Result<GetArtistsResponse>(new InvalidOperationException($"Artist '{streamId}' was null"));
            list.Add(artist);
        }

        return new GetArtistsResponse(list);
    }
}