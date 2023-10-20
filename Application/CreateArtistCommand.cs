using DotNext;
using FluentValidation;
using FolkLibrary.Services;
using Marten;
using MediatR;

namespace FolkLibrary;

public sealed record class CreateArtistCommand(
    string Name,
    string ShortName,
    string? Description,
    int? Year,
    bool IsYearUncertain,
    Location Location
)
    : IRequest<Result<Guid>>;

public sealed class CreateArtistCommandValidator : AbstractValidator<CreateArtistCommand>
{
    public CreateArtistCommandValidator(IValidator<Location> locationValidator)
    {
        RuleFor(r => r.Name).NotEmpty();
        RuleFor(r => r.ShortName).NotEmpty();
        RuleFor(r => r.Year).InclusiveBetween(1900, 2100).When(r => r.Year is not null);
        RuleFor(r => r.Location).NotEmpty().SetValidator(locationValidator);
    }
}


public sealed class CreateArtistCommandHandler : IRequestHandler<CreateArtistCommand, Result<Guid>>
{
    private readonly IDocumentSession _documentSession;
    private readonly IUuidProvider _uuidProvider;

    public CreateArtistCommandHandler(IDocumentSession documentSession, IUuidProvider uuidProvider)
    {
        _documentSession = documentSession;
        _uuidProvider = uuidProvider;
    }

    public async Task<Result<Guid>> Handle(CreateArtistCommand request, CancellationToken cancellationToken)
    {
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