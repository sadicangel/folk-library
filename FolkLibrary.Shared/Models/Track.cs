using System.Diagnostics;

namespace FolkLibrary.Models;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Track : Item, IEquatable<Track>, IComparable<Track>
{
    public int Number { get; set; }

    public TimeSpan Duration { get; set; }

    public Album Album { get; set; } = null!;

    public HashSet<Artist> Artists { get; set; } = new();

    private string GetDebuggerDisplay() => $"{Number:D2} {Name} ({Duration:mm\\:ss})";
    public bool Equals(Track? other) => base.Equals(other);
    public override bool Equals(object? obj) => base.Equals(obj as Track);
    public override int GetHashCode() => base.GetHashCode();
    public int CompareTo(Track? other) => base.CompareTo(other);
}
