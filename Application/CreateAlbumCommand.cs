using DotNext;
using FluentValidation;
using FolkLibrary.Services;
using Marten;
using MediatR;

namespace FolkLibrary;

public sealed record class CreateAlbumCommand(
    string Name,
    string? Description,
    int? Year,
    List<Track> Tracks
)
    : IRequest<Result<Guid>>;

public sealed class CreateAlbumCommandValidator : AbstractValidator<CreateAlbumCommand>
{
    public CreateAlbumCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Year).InclusiveBetween(1900, 2100).When(x => x.Year is not null);
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
        var albumCreated = new AlbumCreated(
            AlbumId: await _uuidProvider.ProvideUuidAsync(cancellationToken),
            Name: request.Name,
            Description: request.Description,
            Year: request.Year,
            Tracks: request.Tracks);

        _documentSession.Events.StartStream<Album>(albumCreated.AlbumId, albumCreated);
        await _documentSession.SaveChangesAsync(cancellationToken);

        return albumCreated.AlbumId;
    }
}
