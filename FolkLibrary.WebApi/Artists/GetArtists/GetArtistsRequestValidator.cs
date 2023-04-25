using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;

namespace FolkLibrary.Artists.GetArtists;

public sealed class GetArtistsRequestValidator : Validator<GetArtistsRequest>
{
    public GetArtistsRequestValidator(ISystemClock systemClock)
    {
        When(e => e.PageIndex.HasValue, () => RuleFor(e => e.PageIndex).GreaterThan(0));
        When(e => e.PageSize.HasValue, () => RuleFor(e => e.PageSize).GreaterThan(0));
        When(e => e.Filter is not null, () => RuleFor(e => e.Filter).ChildRules(v =>
        {
            v.When(e => e!.Country is not null, () => v.RuleFor(e => e!.Country).Length(2));
            v.When(e => e!.AfterYear is not null, () => v.RuleFor(e => e!.AfterYear).GreaterThan(1900).LessThanOrEqualTo(systemClock.UtcNow.Year));
            v.When(e => e!.BeforeYear is not null, () => v.RuleFor(e => e!.BeforeYear).GreaterThan(1900).LessThanOrEqualTo(systemClock.UtcNow.Year));
        }));
    }
}
