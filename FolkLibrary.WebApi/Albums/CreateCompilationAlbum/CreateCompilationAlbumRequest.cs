using FolkLibrary.Tracks;

namespace FolkLibrary.Albums.CreateCompilationAlbum;

public sealed class CreateCompilationAlbumRequest
{
    public required Dictionary<string, List<int>> TracksByArtistId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public int? Year { get; init; }
    public required HashSet<string> Genres { get; init; }
    public required List<CreateTrackDto> Tracks { get; init; }
}