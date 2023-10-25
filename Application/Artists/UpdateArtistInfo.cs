using DotNext;
using FluentValidation;
using Marten;
using MediatR;

namespace FolkLibrary.Artists;

public sealed record class UpdateArtistInfoRequest(
    string? Name,
    string? ShortName,
    string? Description,
    int? Year,
    bool? IsYearUncertain);

public sealed record class UpdateArtistInfo(Guid ArtistId, UpdateArtistInfoRequest Request) : IRequest<Result<Unit>>;

public sealed class UpdateArtistInfoValidator : AbstractValidator<UpdateArtistInfo>
{
    public UpdateArtistInfoValidator()
    {
        RuleFor(r => r.ArtistId).NotEmpty();
        RuleFor(r => r.Request).NotEmpty().ChildRules(v =>
        {
            v.RuleFor(r => r.Name).NotEmpty();
            v.RuleFor(r => r.ShortName).NotEmpty();
            v.RuleFor(r => r.Year).InclusiveBetween(1900, 2100).When(r => r.Year is not null);
        });
    }
}


public sealed class UpdateArtistInfoHandler : IRequestHandler<UpdateArtistInfo, Result<Unit>>
{
    private readonly IValidator<UpdateArtistInfo> _validator;
    private readonly IDocumentSession _documentSession;

    public UpdateArtistInfoHandler(IValidator<UpdateArtistInfo> validator, IDocumentSession documentSession)
    {
        _validator = validator;
        _documentSession = documentSession;
    }

    public async Task<Result<Unit>> Handle(UpdateArtistInfo request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new Result<Unit>(new ValidationException(validationResult.Errors));

        var artistUpdated = new ArtistInfoUpdated(
            request.Request.Name,
            request.Request.ShortName,
            request.Request.Description,
            request.Request.Year,
            request.Request.IsYearUncertain);

        await _documentSession.Events.AppendOptimistic(request.ArtistId, artistUpdated);
        await _documentSession.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}