using FastEndpoints;

namespace FolkLibrary.Artists.CreateArtist;

public sealed class CreateArtistMapper : Mapper<CreateArtistRequest, ArtistDto, Artist>
{
    public override Task<Artist> ToEntityAsync(CreateArtistRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new Artist
        {
            Name = request.Name,
            ShortName = request.ShortName,
            Description = request.Description,
            Year = request.Year,
            IsYearUncertain = request.IsYearUncertain,
            Genres = request.Genres,
            Country = request.Country,
            District = request.District,
            Municipality = request.Municipality,
            Parish = request.Parish,
            IsAbroad = request.IsAbroad,
        });
    }

    public override Task<ArtistDto> FromEntityAsync(Artist entity, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(entity.ToArtistDto());
    }
}
