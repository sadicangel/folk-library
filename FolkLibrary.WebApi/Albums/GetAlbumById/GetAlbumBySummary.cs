using FastEndpoints;
using FolkLibrary.Tracks;

namespace FolkLibrary.Albums.GetAlbumById;

public sealed class GetAlbumBySummary : Summary<GetAlbumByIdEndpoint>
{
    public GetAlbumBySummary()
    {
        Summary = "Gets a Album by Id";
        Description = "Gets a Album by Id";
        ExampleRequest = new GetAlbumByIdRequest { AlbumId = Guid.Empty.ToString() };
        Response(200, example: new AlbumDto
        {
            Id = Guid.Empty.ToString(),
            Name = "Album Name",
            Description = "Optional album description",
            Genres = new List<string> { "Folk" },
            Year = 1999,
            YearString = "1999",
            Duration = TimeSpan.FromMinutes(3),
            TrackCount = 2,
            Tracks = new List<TrackDto>
            {
                new TrackDto
                {
                    Id = Guid.Empty.ToString(),
                    Name = "Track 1",
                    Number = 1,
                    Description = "Optional track 2 description",
                    Duration = TimeSpan.FromMinutes(1),
                    Genres = new List<string> { "Folk" },
                    Year = 1999,
                },
                new TrackDto
                {
                    Id = Guid.Empty.ToString(),
                    Name = "Track 2",
                    Number = 2,
                    Description = "Optional track 1 description",
                    Duration = TimeSpan.FromMinutes(2),
                    Genres = new List<string> { "Folk" },
                    Year = 1999,
                }
            }
        });
        Response<ErrorResponse>(400);
        Response(404, "Not found");
    }
}
