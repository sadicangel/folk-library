﻿using FastEndpoints;
using FolkLibrary.Albums;

namespace FolkLibrary.Artists.UpdateArtist;

public sealed class UpdateArtistSummary : Summary<UpdateArtistEndpoint>
{
    public UpdateArtistSummary()
    {
        Summary = "Updates an Artist";
        Description = "Updates an existing Artist";
        ExampleRequest = new UpdateArtistRequest
        {
            ArtistId = Guid.Empty.ToString(),
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
