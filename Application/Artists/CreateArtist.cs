using DotNext;
using FluentValidation;
using FolkLibrary.Services;
using Marten;
using MediatR;

namespace FolkLibrary.Artists;

public sealed record class CreateArtist(
    string Name,
    string ShortName,
    string? Description,
    int? Year,
    bool IsYearUncertain,
    Location Location
)
    : IRequest<Result<Guid>>;

public sealed class CreateArtistValidator : AbstractValidator<CreateArtist>
{
    public CreateArtistValidator(IValidator<Location> locationValidator)
    {
        RuleFor(r => r.Name).NotEmpty();
        RuleFor(r => r.ShortName).NotEmpty();
        RuleFor(r => r.Year).InclusiveBetween(1900, 2100).When(r => r.Year is not null);
        RuleFor(r => r.Location).NotEmpty().SetValidator(locationValidator);
    }
}


public sealed class CreateArtistHandler : IRequestHandler<CreateArtist, Result<Guid>>
{
    private readonly IValidator<CreateArtist> _validator;
    private readonly IDocumentSession _documentSession;
    private readonly IUuidProvider _uuidProvider;

    public CreateArtistHandler(IValidator<CreateArtist> validator, IDocumentSession documentSession, IUuidProvider uuidProvider)
    {
        _validator = validator;
        _documentSession = documentSession;
        _uuidProvider = uuidProvider;
    }

    public async Task<Result<Guid>> Handle(CreateArtist request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new Result<Guid>(new ValidationException(validationResult.Errors));

        var artistId = await _uuidProvider.ProvideUuidAsync(cancellationToken);
        var artisCreated = new ArtistCreated(
            ArtistId: artistId,
            Name: request.Name,
            ShortName: request.ShortName,
            Description: request.Description,
            Year: request.Year,
            IsYearUncertain: request.IsYearUncertain,
            Location: request.Location);

        _documentSession.Events.StartStream(artistId, artisCreated);
        await _documentSession.SaveChangesAsync(cancellationToken);
        return artistId;
    }
}