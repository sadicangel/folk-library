using DotNext;
using FluentValidation;
using Marten;
using MediatR;

namespace FolkLibrary;

public sealed record class UpdateArtistInfoRequest(
    string? Name,
    string? ShortName,
    string? Description,
    int? Year,
    bool? IsYearUncertain);

public sealed record class UpdateArtistInfoCommand(Guid ArtistId, UpdateArtistInfoRequest Request) : IRequest<Result<Unit>>;

public sealed class UpdateArtistInfoRequestValidator : AbstractValidator<UpdateArtistInfoCommand>
{
    public UpdateArtistInfoRequestValidator()
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


public sealed class UpdateArtistInfoRequestHandler : IRequestHandler<UpdateArtistInfoCommand, Result<Unit>>
{
    private readonly IDocumentSession _documentSession;

    public UpdateArtistInfoRequestHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<Result<Unit>> Handle(UpdateArtistInfoCommand request, CancellationToken cancellationToken)
    {
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