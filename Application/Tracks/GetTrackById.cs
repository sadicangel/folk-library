using DotNext;
using FluentValidation;
using Marten;
using MediatR;

namespace FolkLibrary.Tracks;

public sealed record class GetTrackById(Guid TrackId) : IRequest<Result<Optional<Track>>>;

public sealed class GetTrackByIdValidator : AbstractValidator<GetTrackById>
{
    public GetTrackByIdValidator()
    {
        RuleFor(r => r.TrackId).NotEmpty();
    }
}

public sealed class GetTrackByIdHandler : IRequestHandler<GetTrackById, Result<Optional<Track>>>
{
    private readonly IValidator<GetTrackById> _validator;
    private readonly IDocumentSession _documentSession;

    public GetTrackByIdHandler(IValidator<GetTrackById> validator, IDocumentSession documentSession)
    {
        _validator = validator;
        _documentSession = documentSession;
    }

    public async Task<Result<Optional<Track>>> Handle(GetTrackById request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new Result<Optional<Track>>(new ValidationException(validationResult.Errors));

        return new Optional<Track>(await _documentSession.Events.AggregateStreamAsync<Track>(request.TrackId, token: cancellationToken));
    }
}