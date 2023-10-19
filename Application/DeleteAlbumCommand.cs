using DotNext;
using FluentValidation;
using Marten;
using MediatR;

namespace FolkLibrary;

public sealed record class DeleteAlbumCommand(Guid AlbumId, List<Guid> ArtistIds) : IRequest<Result<Unit>>;

public sealed class DeleteAlbumCommandValidator : AbstractValidator<DeleteAlbumCommand>
{
    public DeleteAlbumCommandValidator()
    {
        RuleFor(x => x.AlbumId).NotEmpty();
        RuleFor(x => x.ArtistIds).NotEmpty();
    }
}

public sealed class DeleteAlbumCommandHandler : IRequestHandler<DeleteAlbumCommand, Result<Unit>>
{
    private readonly IDocumentSession _documentSession;

    public DeleteAlbumCommandHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<Result<Unit>> Handle(DeleteAlbumCommand request, CancellationToken cancellationToken)
    {
        var albumDeleted = new AlbumDeleted(request.AlbumId);

        foreach (var artistId in request.ArtistIds)
            await _documentSession.Events.AppendOptimistic(artistId, cancellationToken, albumDeleted);

        await _documentSession.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}