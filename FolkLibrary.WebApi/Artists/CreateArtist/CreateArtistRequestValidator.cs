using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;

namespace FolkLibrary.Artists.CreateArtist;

public sealed class CreateArtistRequestValidator : Validator<CreateArtistRequest>
{
    public CreateArtistRequestValidator(ISystemClock systemClock)
    {
        RuleFor(e => e.Name).NotEmpty();
        RuleFor(e => e.ShortName).NotEmpty();
        RuleFor(e => e.Description).MaximumLength(byte.MaxValue);
        RuleFor(e => e.Year).NotEmpty().GreaterThan(1900).LessThanOrEqualTo(systemClock.UtcNow.Year);
        RuleFor(e => e.Genres).NotEmpty();
        RuleFor(e => e.Country).NotEmpty().Length(exactLength: 2);
    }
}
