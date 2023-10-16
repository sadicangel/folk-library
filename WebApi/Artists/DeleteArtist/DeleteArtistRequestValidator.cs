using FastEndpoints;
using FluentValidation;

namespace FolkLibrary.Artists.DeleteArtist;

public sealed class DeleteArtistRequestValidator : Validator<DeleteArtistRequest>
{
    public DeleteArtistRequestValidator()
    {
        RuleFor(e => e.ArtistId).NotEmpty();
    }
}
