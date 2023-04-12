using FolkLibrary.Albums;
using FolkLibrary.Artists;
using System.Diagnostics;

namespace FolkLibrary.Tracks;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Track : Entity
{
    public required int Number { get; set; }

    public required TimeSpan Duration { get; set; }

    public required Album Album { get; set; }

    public HashSet<Artist> Artists { get; set; } = new();

    private string GetDebuggerDisplay() => $"{Number:D2} {Name} ({Duration:mm\\:ss})";
}
