using FastEndpoints;
using FolkLibrary.Repositories;
using Mapster;

namespace FolkLibrary.Artists.UpdateArtist;

public sealed class UpdateArtistMapper : Mapper<UpdateArtistRequest, ArtistDto, Artist?>
{
    private static readonly TypeAdapterConfig AdapterConfig = new()
    {
        Default =
        {
            Settings =
            {
                IgnoreNullValues = true,
            }
        }
    };

    public override Task<ArtistDto> FromEntityAsync(Artist? entity, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(entity?.Adapt<ArtistDto>() ?? throw new ArgumentNullException(nameof(entity)));
    }

    public override async Task<Artist?> ToEntityAsync(UpdateArtistRequest request, CancellationToken cancellationToken = default)
    {
        var artist = await Resolve<IArtistRepository>().GetByIdAsync(request.ArtistId, cancellationToken);
        if (artist is not null)
            request.Adapt(artist, AdapterConfig);
        return artist;
    }
}
