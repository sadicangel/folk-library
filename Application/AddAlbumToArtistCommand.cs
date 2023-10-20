using DotNext;
using FluentValidation;
using Marten;
using MediatR;

namespace FolkLibrary;

public sealed record class AddAlbumToArtistCommand(Guid ArtistId, Guid AlbumId) : IRequest<Result<Unit>>;

public sealed class AddAlbumToArtistCommandValidator : AbstractValidator<AddAlbumToArtistCommand>
{
    public AddAlbumToArtistCommandValidator()
    {
        RuleFor(r => r.ArtistId).NotEmpty();
        RuleFor(r => r.AlbumId).NotEmpty();
    }
}

public sealed class AddAlbumToArtistCommandHandler : IRequestHandler<AddAlbumToArtistCommand, Result<Unit>>
{
    private readonly IDocumentSession _documentSession;

    public AddAlbumToArtistCommandHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<Result<Unit>> Handle(AddAlbumToArtistCommand request, CancellationToken cancellationToken)
    {
        var album = await _documentSession.Events.AggregateStreamAsync<Album>(request.AlbumId, token: cancellationToken);
        if (album is null)
            return new Result<Unit>(new FolkLibraryException($"Album {request.AlbumId} does not exist"));

        await _documentSession.Events.AppendOptimistic(request.ArtistId, new AlbumAddedToArtist(album));
        await _documentSession.Events.AppendOptimistic(request.AlbumId, new ArtistAddedToAlbum(request.ArtistId));

        await _documentSession.SaveChangesAsync();

        return Unit.Value;
    }
}
