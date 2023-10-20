using DotNext;
using FluentValidation;
using Marten;
using MediatR;

namespace FolkLibrary;

public sealed record class UpdateAlbumRequest(string Name, string? Description, int? Year);

public sealed record class UpdateAlbumCommand(Guid AlbumId, UpdateAlbumRequest Request) : IRequest<Result<Unit>>;

public sealed class UpdateAlbumCommandValidator : AbstractValidator<UpdateAlbumCommand>
{
    public UpdateAlbumCommandValidator()
    {
        RuleFor(x => x.AlbumId).NotEmpty();
        RuleFor(x => x.Request).NotEmpty().ChildRules(v =>
        {
            v.RuleFor(x => x.Name).NotEmpty();
            v.RuleFor(x => x.Year).InclusiveBetween(1900, 2100).When(x => x.Year is not null);
        });
    }
}

public sealed class UpdateAlbumCommandHandler : IRequestHandler<UpdateAlbumCommand, Result<Unit>>
{
    private readonly IDocumentSession _documentSession;

    public UpdateAlbumCommandHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<Result<Unit>> Handle(UpdateAlbumCommand request, CancellationToken cancellationToken)
    {
        var album = await _documentSession.Events.AggregateStreamAsync<Album>(request.AlbumId, token: cancellationToken);
        if (album is null)
            return new Result<Unit>(new FolkLibraryException($"Album {request.AlbumId} does not exist"));

        var albumUpdated = new AlbumUpdated(
            AlbumId: request.AlbumId,
            Name: request.Request.Name,
            Description: request.Request.Description,
            Year: request.Request.Year);

        await _documentSession.Events.AppendOptimistic(request.AlbumId, albumUpdated);

        foreach (var artistId in album.Artists)
            await _documentSession.Events.AppendOptimistic(artistId, albumUpdated);

        await _documentSession.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}