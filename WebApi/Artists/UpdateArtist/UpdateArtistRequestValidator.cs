using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;

namespace FolkLibrary.Artists.UpdateArtist;

public sealed class UpdateArtistRequestValidator : Validator<UpdateArtistRequest>
{
    public UpdateArtistRequestValidator(ISystemClock systemClock)
    {
        When(e => e.Description is not null, () => RuleFor(e => e.Description).MaximumLength(byte.MaxValue));
        When(e => e.Year.HasValue, () => RuleFor(e => e.Year).NotEmpty().GreaterThan(1900).LessThanOrEqualTo(systemClock.UtcNow.Year));
        When(e => e.Genres is not null, () => RuleFor(e => e.Genres).NotEmpty());
        When(e => e.Country is not null, () => RuleFor(e => e.Country).NotEmpty().Length(exactLength: 2));
    }
}
