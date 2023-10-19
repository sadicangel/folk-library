using DotNext;
using FluentValidation;
using Marten;
using MediatR;

namespace FolkLibrary;

public sealed record class UpdateArtistRequest(
    string? Name,
    string? ShortName,
    string? Description,
    int? Year,
    bool? IsYearUncertain,
    bool? IsAbroad,
    string? Country,
    string? District,
    string? Municipality,
    string? Parish);

public sealed record class UpdateArtistCommand(Guid ArtistId, UpdateArtistRequest Request) : IRequest<Result<Unit>>;

public sealed class UpdateArtistRequestValidator : AbstractValidator<UpdateArtistCommand>
{
    public UpdateArtistRequestValidator()
    {
        RuleFor(r => r.ArtistId).NotEmpty();
        RuleFor(r => r.Request).NotEmpty().ChildRules(v =>
        {
            v.RuleFor(r => r.Name).NotEmpty();
            v.RuleFor(r => r.ShortName).NotEmpty();
            v.RuleFor(r => r.Year).InclusiveBetween(1900, 2100).When(r => r.Year is not null);
            v.RuleFor(r => r.IsAbroad).Must(v => v is true).When(r => r.Country is not "PT");
            v.RuleFor(r => r.Country).NotEmpty();
        });
    }
}


public sealed class UpdateArtistRequestHandler : IRequestHandler<UpdateArtistCommand, Result<Unit>>
{
    private readonly IDocumentSession _documentSession;

    public UpdateArtistRequestHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<Result<Unit>> Handle(UpdateArtistCommand request, CancellationToken cancellationToken)
    {
        var artistUpdated = new ArtistUpdated(
            request.Request.Name,
            request.Request.ShortName,
            request.Request.Description,
            request.Request.Year,
            request.Request.IsYearUncertain,
            request.Request.IsAbroad,
            request.Request.Country,
            request.Request.District,
            request.Request.Municipality,
            request.Request.Parish);

        await _documentSession.Events.AppendOptimistic(request.ArtistId, artistUpdated);
        await _documentSession.SaveChangesAsync();

        return Unit.Value;
    }
}