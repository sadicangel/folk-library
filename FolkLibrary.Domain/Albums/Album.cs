using FolkLibrary.Artists;
using FolkLibrary.Tracks;
using System.Diagnostics;

namespace FolkLibrary.Albums;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Album : Entity
{

    public required TimeSpan Duration { get; set; }

    public bool IsIncomplete { get; set; }

    public int ArtistCount { get => Artists.Count; }

    public HashSet<Artist> Artists { get; init; } = new();

    public int TrackCount { get => Tracks.Count; }

    public List<Track> Tracks { get; init; } = new();

    private string GetDebuggerDisplay() => $"{Name} ({Duration:mm\\:ss})";
}