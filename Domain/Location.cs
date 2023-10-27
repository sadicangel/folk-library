using FluentValidation;

namespace FolkLibrary;

public sealed record class Location(
    string CountryCode,
    string CountryName,
    string District,
    string? Municipality,
    string? Parish
);

public sealed class LocationValidator : AbstractValidator<Location>
{
    public LocationValidator()
    {
        RuleFor(r => r.CountryCode).NotEmpty().Length(2);
        RuleFor(r => r.CountryName).NotEmpty();
        RuleFor(r => r.District).NotEmpty();
    }
}