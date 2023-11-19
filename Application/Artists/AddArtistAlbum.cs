using DotNext;
using FluentValidation;
using FolkLibrary.Albums;
using Marten;
using MediatR;

namespace FolkLibrary.Artists;

public sealed record class AddArtistAlbum(Guid ArtistId, Guid AlbumId) : IRequest<Result<Unit>>;

public sealed class AddArtistAlbumValidator : AbstractValidator<AddArtistAlbum>
{
    public AddArtistAlbumValidator()
    {
        RuleFor(r => r.ArtistId).NotEmpty();
        RuleFor(r => r.AlbumId).NotEmpty();
    }
}

public sealed class AddArtistAlbumHandler(IValidator<AddArtistAlbum> validator, IDocumentSession documentSession) : IRequestHandler<AddArtistAlbum, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(AddArtistAlbum request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new Result<Unit>(new ValidationException(validationResult.Errors));

        var album = await documentSession.Events.AggregateStreamAsync<Album>(request.AlbumId, token: cancellationToken);
        if (album is null)
            return new Result<Unit>(new FolkLibraryException($"Album {request.AlbumId} does not exist"));

        var albumArtistAdded = new AlbumArtistAdded(request.ArtistId);

        await documentSession.Events.AppendOptimistic(request.AlbumId, albumArtistAdded);

        // TODO: Make this a notification instead?
        album = albumArtistAdded.Apply(album);

        await documentSession.Events.AppendOptimistic(request.ArtistId, new ArtistAlbumAdded(album));

        await documentSession.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
