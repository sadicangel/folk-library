using FastEndpoints;
using FluentValidation;

namespace FolkLibrary.Albums.GetAlbumById;

public sealed class GetAlbumByIdRequestValidator : Validator<GetAlbumByIdRequest>
{
    public GetAlbumByIdRequestValidator() => RuleFor(e => e.AlbumId).NotEmpty();
}
