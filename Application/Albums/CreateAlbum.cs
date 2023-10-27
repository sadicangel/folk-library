using DotNext;
using FluentValidation;
using FolkLibrary.Services;
using Marten;
using MediatR;

namespace FolkLibrary.Albums;

public sealed record class CreateAlbum(
    string Name,
    string? Description,
    int? Year
)
    : IRequest<Result<Guid>>;

public sealed class CreateAlbumValidator : AbstractValidator<CreateAlbum>
{
    public CreateAlbumValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Year).InclusiveBetween(1900, 2100).When(x => x.Year is not null);
    }
}

public sealed class CreateAlbumHandler : IRequestHandler<CreateAlbum, Result<Guid>>
{
    private readonly IValidator<CreateAlbum> _validator;
    private readonly IDocumentSession _documentSession;
    private readonly IUuidProvider _uuidProvider;

    public CreateAlbumHandler(IValidator<CreateAlbum> validator, IDocumentSession documentSession, IUuidProvider uuidProvider)
    {
        _validator = validator;
        _documentSession = documentSession;
        _uuidProvider = uuidProvider;
    }

    public async Task<Result<Guid>> Handle(CreateAlbum request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new Result<Guid>(new ValidationException(validationResult.Errors));

        var albumCreated = new AlbumCreated(
            AlbumId: await _uuidProvider.ProvideUuidAsync(cancellationToken),
            Name: request.Name,
            Description: request.Description,
            Year: request.Year);

        _documentSession.Events.StartStream<Album>(albumCreated.AlbumId, albumCreated);
        await _documentSession.SaveChangesAsync(cancellationToken);

        return albumCreated.AlbumId;
    }
}
