using FastEndpoints;
using FolkLibrary.Albums;

namespace FolkLibrary.Artists.GetArtists;

public sealed class GetArtistsSummanry : Summary<GetArtistsEndpoint>
{
    public GetArtistsSummanry()
    {
        Summary = "Gets a page of Artists";
        Description = "Gets a page of Artists";
        ExampleRequest = new GetArtistsRequest
        {
            PageSize = 10,
            PageIndex = 2,
            Filter = new ArtistFilterDto
            {
                BeforeYear = 2000,
                Municipality = "Ponte de Lima"
            }
        };
        Response(200, example: new Page<ArtistDto>(2, false, new List<ArtistDto>
        {
            new ArtistDto
            {
                Id = Guid.Empty.ToString(),
                Name = "Artist Full Name",
                ShortName = "Artist Short Name",
                LetterAvatar = "ASN",
                Description = "Optional artist description",
                Genres = new List<string> { "Folk" },
                Year = 1999,
                YearString = "1999?",
                IsYearUncertain = true,
                Country = "PT",
                District = "Viana do Castelo",
                Municipality = "Ponte de Lima",
                Parish = "Estorãos",
                Location = "Viana do Castelo, Ponte de Lima, Estorãos",
                IsAbroad = false,
                Albums = new List<AlbumDto>()
            }
        }));
        Response<ErrorResponse>(400);
    }
}
