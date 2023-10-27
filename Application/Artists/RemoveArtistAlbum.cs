using DotNext;
using FluentValidation;
using FolkLibrary.Albums;
using Marten;
using MediatR;

namespace FolkLibrary.Artists;

public sealed record class RemoveArtistAlbum(Guid ArtistId, Guid AlbumId) : IRequest<Result<Unit>>;

public sealed class RemoveArtistAlbumValidator : AbstractValidator<RemoveArtistAlbum>
{
    public RemoveArtistAlbumValidator()
    {
        RuleFor(r => r.ArtistId).NotEmpty();
        RuleFor(r => r.AlbumId).NotEmpty();
    }
}

public sealed class RemoveArtistAlbumHandler : IRequestHandler<RemoveArtistAlbum, Result<Unit>>
{
    private readonly IValidator<RemoveArtistAlbum> _validator;
    private readonly IDocumentSession _documentSession;

    public RemoveArtistAlbumHandler(IValidator<RemoveArtistAlbum> validator, IDocumentSession documentSession)
    {
        _validator = validator;
        _documentSession = documentSession;
    }

    public async Task<Result<Unit>> Handle(RemoveArtistAlbum request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new Result<Unit>(new ValidationException(validationResult.Errors));

        await _documentSession.Events.AppendOptimistic(request.ArtistId, new ArtistAlbumRemoved(request.AlbumId));
        await _documentSession.Events.AppendOptimistic(request.AlbumId, new AlbumArtistRemoved(request.ArtistId));

        await _documentSession.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}