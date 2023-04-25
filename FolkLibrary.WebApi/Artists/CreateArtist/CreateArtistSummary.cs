using FastEndpoints;
using FolkLibrary.Albums;

namespace FolkLibrary.Artists.CreateArtist;

public sealed class CreateArtistSummary : Summary<CreateArtistEndpoint>
{
    public CreateArtistSummary()
    {
        Summary = "Creates a new Artist";
        Description = "Creates a new Artist";
        ExampleRequest = new CreateArtistRequest
        {
            Name = "Artist Full Name",
            ShortName = "Artist Short Name",
            Description = "Optional artist description",
            Genres = new HashSet<string> { "Folk" },
            Year = 1999,
            IsYearUncertain = true,
            Country = "PT",
            District = "Viana do Castelo",
            Municipality = "Ponte de Lima",
            Parish = "Estorãos",
            IsAbroad = false,
        };
        Response(201, example: new ArtistDto
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
        });
        Response<ErrorResponse>(400);
    }
}
