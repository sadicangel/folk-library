using FastEndpoints;
using FluentValidation;

namespace FolkLibrary.Artists.GetArtistById;

public sealed class GetArtistByIdRequestValidator : Validator<GetArtistByIdRequest>
{
    public GetArtistByIdRequestValidator() => RuleFor(e => e.ArtistId).NotEmpty();
}
