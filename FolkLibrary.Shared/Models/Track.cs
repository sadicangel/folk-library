using System.Diagnostics;

namespace FolkLibrary.Models;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Track : Item, IEquatable<Track>, IComparable<Track>
{
    public int Number { get; set; }
    public TimeSpan Duration { get; set; }
    public int? Year { get; set; }
    public Album? Album { get; set; } = null!;
    public HashSet<Artist> Artists { get; set; } = null!;
    public HashSet<Genre> Genres { get; set; } = null!;

    private string GetDebuggerDisplay() => $"{Number:D2} {Name} ({Duration:mm\\:ss})";
    public bool Equals(Track? other) => other is not null && Id == other.Id;
    public override bool Equals(object? obj) => Equals(obj as Track);
    public override int GetHashCode() => Id.GetHashCode();
    public int CompareTo(Track? other) => other is null ? 1 : Name.CompareTo(other.Name);

    public static bool operator ==(Track left, Track right) => left is null ? right is null : left.Equals(right);
    public static bool operator !=(Track left, Track right) => !(left == right);
    public static bool operator <(Track left, Track right) => left is null ? right is not null : left.CompareTo(right) < 0;
    public static bool operator <=(Track left, Track right) => left is null || left.CompareTo(right) <= 0;
    public static bool operator >(Track left, Track right) => left is not null && left.CompareTo(right) > 0;
    public static bool operator >=(Track left, Track right) => left is null ? right is null : left.CompareTo(right) >= 0;
}
