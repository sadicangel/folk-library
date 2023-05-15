using FastEndpoints;
using FolkLibrary.Albums;

namespace FolkLibrary.Artists.GetArtistById;

public sealed class GetArtistByIdSummary : Summary<GetArtistByIdEndpoint>
{
    public GetArtistByIdSummary()
    {
        Summary = "Gets an Artist";
        Description = "Gets an Artist";
        ExampleRequest = new GetArtistByIdRequest
        {
            ArtistId = Guid.Empty.ToString()
        };
        Response(200, example: new ArtistDto
        {
            Id = Guid.Empty.ToString(),
            Name = "Artist Full Name",
            ShortName = "Artist Short Name",
            LetterAvatar = "ASN",
            Description = "Optional artist description",
            Genres = new HashSet<string> { "Folk" },
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
        });
        Response<ErrorResponse>(400);
    }
}
