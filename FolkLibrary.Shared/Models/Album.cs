using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FolkLibrary.Models;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Album : Item, IEquatable<Album>, IComparable<Album>
{
    public int TrackCount { get; set; }

    public TimeSpan Duration { get; set; }

    public bool IsIncomplete { get; set; }

    public HashSet<Artist> Artists { get; set; } = new();

    public List<Track> Tracks { get; set; } = new();

    private string GetDebuggerDisplay() => $"{Name} ({Duration:mm\\:ss})";
    public bool Equals(Album? other) => base.Equals(other);
    public override bool Equals(object? obj) => base.Equals(obj as Album);
    public override int GetHashCode() => base.GetHashCode();
    public int CompareTo(Album? other) => base.CompareTo(other);
}