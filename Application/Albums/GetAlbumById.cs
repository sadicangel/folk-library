using DotNext;
using FluentValidation;
using Marten;
using MediatR;

namespace FolkLibrary.Albums;

public sealed record class GetAlbumById(Guid AlbumId) : IRequest<Result<Optional<Album>>>;

public sealed class GetAlbumByIdValidator : AbstractValidator<GetAlbumById>
{
    public GetAlbumByIdValidator()
    {
        RuleFor(r => r.AlbumId).NotEmpty();
    }
}

public sealed class GetAlbumByIdHandler : IRequestHandler<GetAlbumById, Result<Optional<Album>>>
{
    private readonly IValidator<GetAlbumById> _validator;
    private readonly IDocumentSession _documentSession;

    public GetAlbumByIdHandler(IValidator<GetAlbumById> validator, IDocumentSession documentSession)
    {
        _validator = validator;
        _documentSession = documentSession;
    }

    public async Task<Result<Optional<Album>>> Handle(GetAlbumById request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new Result<Optional<Album>>(new ValidationException(validationResult.Errors));

        return new Optional<Album>(await _documentSession.Events.AggregateStreamAsync<Album>(request.AlbumId, token: cancellationToken));
    }
}
