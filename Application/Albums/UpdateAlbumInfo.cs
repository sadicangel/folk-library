using DotNext;
using FluentValidation;
using Marten;
using MediatR;

namespace FolkLibrary.Albums;

public sealed record class UpdateAlbumInfo(Guid AlbumId, UpdateAlbumInfoRequest Request) : IRequest<Result<Unit>>;

public sealed record class UpdateAlbumInfoRequest(string Name, string? Description, int? Year);

public sealed class UpdateAlbumInfoValidator : AbstractValidator<UpdateAlbumInfo>
{
    public UpdateAlbumInfoValidator()
    {
        RuleFor(x => x.AlbumId).NotEmpty();
        RuleFor(x => x.Request).NotEmpty().ChildRules(v =>
        {
            v.RuleFor(x => x.Name).NotEmpty();
            v.RuleFor(x => x.Year).InclusiveBetween(1900, 2100).When(x => x.Year is not null);
        });
    }
}

public sealed class UpdateAlbumInfoHandler : IRequestHandler<UpdateAlbumInfo, Result<Unit>>
{
    private readonly IValidator<UpdateAlbumInfo> _validator;
    private readonly IDocumentSession _documentSession;
    private readonly IMediator _mediator;

    public UpdateAlbumInfoHandler(IValidator<UpdateAlbumInfo> validator, IDocumentSession documentSession, IMediator mediator)
    {
        _validator = validator;
        _documentSession = documentSession;
        _mediator = mediator;
    }

    public async Task<Result<Unit>> Handle(UpdateAlbumInfo request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new Result<Unit>(new ValidationException(validationResult.Errors));

        var album = await _documentSession.Events.AggregateStreamAsync<Album>(request.AlbumId, token: cancellationToken);
        if (album is null)
            return new Result<Unit>(new FolkLibraryException($"Album {request.AlbumId} does not exist"));

        var albumUpdated = new AlbumInfoUpdated(
            Name: request.Request.Name,
            Description: request.Request.Description,
            Year: request.Request.Year);

        await _documentSession.Events.AppendOptimistic(request.AlbumId, albumUpdated);
        await _documentSession.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(new AlbumUpdatedNotification(album.AlbumId), cancellationToken);

        return Unit.Value;
    }
}
