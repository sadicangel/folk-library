using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;

namespace FolkLibrary.Albums.CreateAlbum;

public sealed class CreateAlbumRequestValidator : Validator<CreateAlbumRequest>
{
    public CreateAlbumRequestValidator(ISystemClock systemClock)
    {
        var now = systemClock.UtcNow;
        RuleFor(e => e.ArtistId).NotEmpty();
        RuleFor(e => e.Name).NotEmpty();
        When(e => e.Year.HasValue, () => RuleFor(e => e.Year).GreaterThan(1900).LessThanOrEqualTo(now.Year));
        RuleFor(e => e.Genres).NotEmpty();
        RuleFor(e => e.Tracks).NotEmpty();
        RuleForEach(e => e.Tracks).ChildRules(v =>
        {
            v.RuleFor(e => e.Name).NotEmpty();
            v.When(e => e.Year.HasValue, () => RuleFor(e => e.Year).GreaterThan(1900).LessThanOrEqualTo(now.Year));
            v.RuleFor(e => e.Genres).NotEmpty();
            v.RuleFor(e => e.Number).GreaterThan(0);
            v.RuleFor(e => e.Duration).GreaterThan(TimeSpan.Zero);
        });
    }
}
