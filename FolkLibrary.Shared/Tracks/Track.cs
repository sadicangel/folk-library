using FolkLibrary.Albums;
using FolkLibrary.Artists;
using System.Diagnostics;

namespace FolkLibrary.Tracks;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Track : Entity<TrackId>
{
    public int Number { get; set; }

    public TimeSpan Duration { get; set; }

    public Album Album { get; set; } = null!;

    public HashSet<Artist> Artists { get; set; } = new();

    private string GetDebuggerDisplay() => $"{Number:D2} {Name} ({Duration:mm\\:ss})";
}
