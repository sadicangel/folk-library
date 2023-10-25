using DotNext;
using FluentValidation;
using Marten;
using MediatR;

namespace FolkLibrary.Artists;

public sealed record class GetArtistById(Guid ArtistId) : IRequest<Result<Optional<Artist>>>;

public sealed class GetArtistByIdValidator : AbstractValidator<GetArtistById>
{
    public GetArtistByIdValidator()
    {
        RuleFor(r => r.ArtistId).NotEmpty();
    }
}

public sealed class GetArtistByIdHandler : IRequestHandler<GetArtistById, Result<Optional<Artist>>>
{
    private readonly IValidator<GetArtistById> _validator;
    private readonly IDocumentSession _documentSession;

    public GetArtistByIdHandler(IValidator<GetArtistById> validator, IDocumentSession documentSession)
    {
        _validator = validator;
        _documentSession = documentSession;
    }

    public async Task<Result<Optional<Artist>>> Handle(GetArtistById request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new Result<Optional<Artist>>(new ValidationException(validationResult.Errors));

        return new Optional<Artist>(await _documentSession.Events.AggregateStreamAsync<Artist>(request.ArtistId, token: cancellationToken));
    }
}