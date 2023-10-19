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
    bool IsAbroad,
    string Country,
    string? District,
    string? Municipality,
    string? Parish,
    List<string> Genres) : IRequest<Result<Guid>>;

public sealed class CreateArtistCommandValidator : AbstractValidator<CreateArtistCommand>
{
    public CreateArtistCommandValidator()
    {
        RuleFor(r => r.Name).NotEmpty();
        RuleFor(r => r.ShortName).NotEmpty();
        RuleFor(r => r.Year).InclusiveBetween(1900, 2100).When(r => r.Year is not null);
        RuleFor(r => r.IsAbroad).Must(v => v is true).When(r => r.Country is not "PT");
        RuleFor(r => r.Country).NotEmpty();
        RuleFor(r => r.Genres).NotEmpty();
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
            artistId,
            request.Name,
            request.ShortName,
            request.Description,
            request.Year,
            request.Year is null,
            request.IsAbroad,
            request.Country,
            request.District,
            request.Municipality,
            request.Parish,
            request.Genres);
        _documentSession.Events.StartStream(artistId, artisCreated);
        await _documentSession.SaveChangesAsync(cancellationToken);
        return artistId;
    }
}