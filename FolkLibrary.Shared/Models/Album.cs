using System.Diagnostics;

namespace FolkLibrary.Models;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Album : Item, IEquatable<Album>, IComparable<Album>
{
    public int? Year { get; set; }
    public int TrackCount { get; set; }
    public TimeSpan Duration { get; set; }
    public HashSet<Artist> Artists { get; set; } = null!;
    public HashSet<Genre> Genres { get; set; } = null!;
    public List<Track> Tracks { get; set; } = null!;

    private string GetDebuggerDisplay() => $"{Name} ({Duration:mm\\:ss})";
    public bool Equals(Album? other) => other is not null && Id == other.Id;
    public override bool Equals(object? obj) => Equals(obj as Album);
    public override int GetHashCode() => Id.GetHashCode();
    public int CompareTo(Album? other) => other is null ? 1 : Name.CompareTo(other.Name);

    public static bool operator ==(Album left, Album right) => left is null ? right is null : left.Equals(right);
    public static bool operator !=(Album left, Album right) => !(left == right);
    public static bool operator <(Album left, Album right) => left is null ? right is not null : left.CompareTo(right) < 0;
    public static bool operator <=(Album left, Album right) => left is null || left.CompareTo(right) <= 0;
    public static bool operator >(Album left, Album right) => left is not null && left.CompareTo(right) > 0;
    public static bool operator >=(Album left, Album right) => left is null ? right is null : left.CompareTo(right) >= 0;
}