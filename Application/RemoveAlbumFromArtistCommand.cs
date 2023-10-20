using DotNext;
using FluentValidation;
using Marten;
using MediatR;

namespace FolkLibrary;

public sealed record class RemoveAlbumFromArtistCommand(Guid ArtistId, Guid AlbumId) : IRequest<Result<Unit>>;

public sealed class RemoveAlbumFromArtistCommandValidator : AbstractValidator<RemoveAlbumFromArtistCommand>
{
    public RemoveAlbumFromArtistCommandValidator()
    {
        RuleFor(r => r.ArtistId).NotEmpty();
        RuleFor(r => r.AlbumId).NotEmpty();
    }
}

public sealed class RemoveAlbumFromArtistCommandHandler : IRequestHandler<RemoveAlbumFromArtistCommand, Result<Unit>>
{
    private readonly IDocumentSession _documentSession;

    public RemoveAlbumFromArtistCommandHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<Result<Unit>> Handle(RemoveAlbumFromArtistCommand request, CancellationToken cancellationToken)
    {
        await _documentSession.Events.AppendOptimistic(request.ArtistId, new AlbumRemovedFromArtist(request.AlbumId));
        await _documentSession.Events.AppendOptimistic(request.AlbumId, new ArtistRemovedFromAlbum(request.ArtistId));

        await _documentSession.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}