using FastEndpoints;
using Mapster;

namespace FolkLibrary.Artists.CreateArtist;

public sealed class CreateArtistMapper : Mapper<CreateArtistRequest, ArtistDto, Artist>
{
    public override Task<Artist> ToEntityAsync(CreateArtistRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(request.Adapt<Artist>());
    }

    public override Task<ArtistDto> FromEntityAsync(Artist entity, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(entity.Adapt<ArtistDto>());
    }
}
