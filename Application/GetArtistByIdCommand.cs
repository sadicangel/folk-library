using DotNext;
using FluentValidation;
using Marten;
using MediatR;

namespace FolkLibrary;

public sealed record class GetArtistByIdCommand(Guid ArtistId) : IRequest<Result<Optional<Artist>>>;

public sealed class GetArtistByIdCommandValidator : AbstractValidator<GetArtistByIdCommand>
{
    public GetArtistByIdCommandValidator()
    {
        RuleFor(r => r.ArtistId).NotEmpty();
    }
}

public sealed class GetArtistByIdCommandHandler : IRequestHandler<GetArtistByIdCommand, Result<Optional<Artist>>>
{
    private readonly IDocumentSession _documentSession;

    public GetArtistByIdCommandHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<Result<Optional<Artist>>> Handle(GetArtistByIdCommand request, CancellationToken cancellationToken)
    {
        return new Optional<Artist>(await _documentSession.Events.AggregateStreamAsync<Artist>(request.ArtistId, token: cancellationToken));
    }
}