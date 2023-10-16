using FastEndpoints;
using FolkLibrary.Repositories;

namespace FolkLibrary.Artists.UpdateArtist;

public sealed class UpdateArtistMapper : Mapper<UpdateArtistRequest, ArtistDto, Artist?>
{
    public override async Task<Artist?> ToEntityAsync(UpdateArtistRequest request, CancellationToken cancellationToken = default)
    {
        var artist = await Resolve<IArtistRepository>().GetByIdAsync(request.ArtistId, cancellationToken);
        if (artist is not null)
        {
            if (request.Name is not null)
                artist.Name = request.Name;
            if (request.ShortName is not null)
                artist.ShortName = request.ShortName;
            if (request.Description is not null)
                artist.Description = request.Description;
            if (request.Year is not null)
                artist.Year = request.Year;
            if (request.IsYearUncertain is not null)
                artist.IsYearUncertain = request.IsYearUncertain.Value;
            if (request.Genres is not null)
                artist.Genres = request.Genres;
            if (request.Country is not null)
                artist.Country = request.Country;
            if (request.District is not null)
                artist.District = request.District;
            if (request.Municipality is not null)
                artist.Municipality = request.Municipality;
            if (request.Parish is not null)
                artist.Parish = request.Parish;
            if (request.IsAbroad is not null)
                artist.IsAbroad = request.IsAbroad.Value;
        }
        return artist;
    }

    public override Task<ArtistDto> FromEntityAsync(Artist? entity, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(entity?.ToArtistDto() ?? throw new ArgumentNullException(nameof(entity)));
    }
}
