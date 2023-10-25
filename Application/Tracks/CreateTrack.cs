using DotNext;
using FluentValidation;
using FolkLibrary.Services;
using Marten;
using MediatR;

namespace FolkLibrary.Tracks;
public sealed record class CreateTrack(string Name, int Number, string? Description, int? Year, TimeSpan Duration) : IRequest<Result<Guid>>;

public sealed class CreateTrackValidator : AbstractValidator<CreateTrack>
{
    public CreateTrackValidator()
    {
        RuleFor(r => r.Name).NotEmpty();
        RuleFor(r => r.Number).NotEmpty().GreaterThan(0);
        RuleFor(r => r.Year).InclusiveBetween(1900, 2100).When(r => r.Year is not null);
        RuleFor(r => r.Duration).NotEmpty().GreaterThan(TimeSpan.Zero);
    }
}

public sealed class CreateTrackHandler : IRequestHandler<CreateTrack, Result<Guid>>
{
    private readonly IValidator<CreateTrack> _validator;
    private readonly IDocumentSession _documentSession;
    private readonly IUuidProvider _uuidProvider;

    public CreateTrackHandler(IValidator<CreateTrack> validator, IDocumentSession documentSession, IUuidProvider uuidProvider)
    {
        _validator = validator;
        _documentSession = documentSession;
        _uuidProvider = uuidProvider;
    }

    public async Task<Result<Guid>> Handle(CreateTrack request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new Result<Guid>(new ValidationException(validationResult.Errors));

        var trackId = await _uuidProvider.ProvideUuidAsync(cancellationToken);
        var trackCreated = new TrackCreated(
            TrackId: trackId,
            Name: request.Name,
            Number: request.Number,
            Description: request.Description,
            Year: request.Year,
            Duration: request.Duration);

        _documentSession.Events.StartStream(trackId, trackCreated);
        await _documentSession.SaveChangesAsync(cancellationToken);
        return trackId;
    }
}
