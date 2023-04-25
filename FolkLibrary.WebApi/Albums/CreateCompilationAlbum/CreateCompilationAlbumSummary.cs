using FastEndpoints;
using FolkLibrary.Tracks;

namespace FolkLibrary.Albums.CreateCompilationAlbum;

public sealed class CreateCompilationAlbumSummary : Summary<CreateCompilationAlbumEndpoint>
{
    public CreateCompilationAlbumSummary()
    {
        Summary = "Creates a new Compilation Album";
        Description = "Creates a new Compilation Album associated to multiple artists";
        ExampleRequest = new CreateCompilationAlbumRequest
        {
            TracksByArtistId = new Dictionary<string, List<int>>
            {
                [Guid.Empty.ToString()] = new List<int> { 1 },
                [Guid.Empty.ToString()] = new List<int> { 2 },
            },
            Name = "Album Name",
            Description = "Optional album description",
            Genres = new HashSet<string> { "Folk" },
            Year = 1999,
            Tracks = new List<CreateTrackDto>
            {
                new CreateTrackDto
                {
                    Name = "Track 1",
                    Number = 1,
                    Description = "Optional track 2 description",
                    Duration = TimeSpan.FromMinutes(1),
                    Genres = new List<string> { "Folk" },
                    Year = 1999,
                },
                new CreateTrackDto
                {
                    Name = "Track 2",
                    Number = 2,
                    Description = "Optional track 1 description",
                    Duration = TimeSpan.FromMinutes(2),
                    Genres = new List<string> { "Folk" },
                    Year = 1999,
                }
            }
        };
        Response(201, example: new AlbumDto
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
    }
}
