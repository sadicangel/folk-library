using DotNext;
using FluentValidation;
using Marten;
using MediatR;

namespace FolkLibrary.Tracks;
public sealed record class UpdateTrackInfo(Guid TrackId, UpdateTrackInfoRequest Request) : IRequest<Result<Unit>>;

public sealed record class UpdateTrackInfoRequest(string? Name, int? Number, string? Description, int? Year, TimeSpan? Duration);

public sealed class UpdateTrackInfoValidator : AbstractValidator<UpdateTrackInfo>
{
    public UpdateTrackInfoValidator()
    {
        RuleFor(r => r.TrackId).NotEmpty();
        RuleFor(r => r.Request).NotEmpty().ChildRules(v =>
        {
            v.RuleFor(r => r.Number).GreaterThan(0).When(v => v.Number is not null);
            v.RuleFor(r => r.Year).InclusiveBetween(1900, 2100).When(r => r.Year is not null);
            v.RuleFor(r => r.Duration).GreaterThan(TimeSpan.Zero).When(r => r.Duration is not null);
        });
    }
}

public sealed class UpdateTrackInfoHandler : IRequestHandler<UpdateTrackInfo, Result<Unit>>
{
    private readonly IValidator<UpdateTrackInfo> _validator;
    private readonly IDocumentSession _documentSession;
    private readonly IMediator _mediator;

    public UpdateTrackInfoHandler(IValidator<UpdateTrackInfo> validator, IDocumentSession documentSession, IMediator mediator)
    {
        _validator = validator;
        _documentSession = documentSession;
        _mediator = mediator;
    }

    public async Task<Result<Unit>> Handle(UpdateTrackInfo request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new Result<Unit>(new ValidationException(validationResult.Errors));

        var track = await _documentSession.Events.AggregateStreamAsync<Track>(request.TrackId, token: cancellationToken);
        if (track is null)
            return new Result<Unit>(new FolkLibraryException($"Track {request.TrackId} does not exist"));

        var trackUpdated = new TrackInfoUpdated(
            Name: request.Request.Name,
            Number: request.Request.Number,
            Description: request.Request.Description,
            Year: request.Request.Year,
            Duration: request.Request.Duration
        );

        await _documentSession.Events.AppendOptimistic(track.TrackId, cancellationToken, trackUpdated);
        await _documentSession.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(new TrackUpdatedNotification(track.TrackId), cancellationToken);

        return Unit.Value;
    }
}
