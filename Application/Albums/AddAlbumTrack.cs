using DotNext;
using FluentValidation;
using FolkLibrary.Services;
using FolkLibrary.Tracks;
using Marten;
using MediatR;

namespace FolkLibrary.Albums;

public sealed record class AddAlbumTrack(
    Guid AlbumId,
    Guid TrackId
) : IRequest<Result<Unit>>;

public sealed class AddAlbumTrackValidator : AbstractValidator<AddAlbumTrack>
{
    public AddAlbumTrackValidator()
    {
        RuleFor(r => r.AlbumId).NotEmpty();
        RuleFor(r => r.TrackId).NotEmpty();
    }
}

public sealed class AddAlbumTrackHandler : IRequestHandler<AddAlbumTrack, Result<Unit>>
{
    private readonly IValidator<AddAlbumTrack> _validator;
    private readonly IDocumentSession _documentSession;
    private readonly IUuidProvider _uuidProvider;

    public AddAlbumTrackHandler(IValidator<AddAlbumTrack> validator, IDocumentSession documentSession, IUuidProvider uuidProvider)
    {
        _validator = validator;
        _documentSession = documentSession;
        _uuidProvider = uuidProvider;
    }

    public async Task<Result<Unit>> Handle(AddAlbumTrack request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new Result<Unit>(new ValidationException(validationResult.Errors));

        var track = await _documentSession.Events.AggregateStreamAsync<Track>(request.TrackId, token: cancellationToken);
        if (track is null)
            return new Result<Unit>(new FolkLibraryException($"Track {request.TrackId} does not exist"));

        var trackAlbumAdded = new TrackAlbumUpdated(request.AlbumId);
        await _documentSession.Events.AppendOptimistic(request.TrackId, cancellationToken, trackAlbumAdded);

        // TODO: Make this a notification instead?
        track = trackAlbumAdded.Apply(track);

        await _documentSession.Events.AppendOptimistic(request.AlbumId, cancellationToken, new AlbumTrackAdded(track!));

        await _documentSession.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}