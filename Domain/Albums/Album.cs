using FolkLibrary.Artists;
using FolkLibrary.Tracks;
using System.Diagnostics;

namespace FolkLibrary.Albums;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Album : Entity
{
    public bool IsCompilation { get; init; }

    public TimeSpan Duration { get => Tracks.Aggregate(TimeSpan.Zero, (p, c) => p + c.Duration); }

    public bool IsIncomplete { get => Tracks.Count == 0 || Tracks.Count != Tracks.Max(t => t.Number); }

    public int ArtistCount { get => Artists.Count; }

    public HashSet<Artist> Artists { get; init; } = new();

    public int TrackCount { get => Tracks.Count; }

    public HashSet<Track> Tracks { get; init; } = new();

    private string GetDebuggerDisplay() => $"{Name} ({Duration:mm\\:ss})";
}