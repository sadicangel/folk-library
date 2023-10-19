using DotNext;
using FluentValidation;
using FolkLibrary.Services;
using Marten;
using MediatR;

namespace FolkLibrary;

public sealed record class CreateAlbumCommand(
    List<string> ArtistIds,
    string Name,
    string? Description,
    int? Year,
    List<string> Genres,
    List<Track> Tracks)
    : IRequest<Result<Guid>>;

public sealed class CreateAlbumCommandValidator : AbstractValidator<CreateAlbumCommand>
{
    public CreateAlbumCommandValidator()
    {
        RuleFor(x => x.ArtistIds).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Year).InclusiveBetween(1900, 2100).When(x => x.Year is not null);
        RuleFor(x => x.Genres).NotEmpty();
        RuleFor(x => x.Tracks).NotEmpty();
    }
}

public sealed class CreateAlbumCommandHandler : IRequestHandler<CreateAlbumCommand, Result<Guid>>
{
    private readonly IDocumentSession _documentSession;
    private readonly IUuidProvider _uuidProvider;

    public CreateAlbumCommandHandler(IDocumentSession documentSession, IUuidProvider uuidProvider)
    {
        _documentSession = documentSession;
        _uuidProvider = uuidProvider;
    }

    public async Task<Result<Guid>> Handle(CreateAlbumCommand request, CancellationToken cancellationToken)
    {
        var albumId = await _uuidProvider.ProvideUuidAsync(cancellationToken);
        var albumCreated = new AlbumCreated(
            albumId,
            request.Name,
            request.Description,
            request.Year,
            IsCompilation: request.ArtistIds.Count > 1,
            request.Genres,
            request.Tracks);

        foreach (var artistId in request.ArtistIds)
            await _documentSession.Events.AppendOptimistic(artistId, cancellationToken, albumCreated);

        await _documentSession.SaveChangesAsync(cancellationToken);

        return albumCreated.AlbumId;
    }
}