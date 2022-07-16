using System.Diagnostics;

namespace FolkLibrary.Models;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Genre : Item, IEquatable<Genre>, IComparable<Genre>
{
    public List<Album> Albums { get; set; } = null!;
    public List<Track> Tracks { get; set; } = null!;

    private string GetDebuggerDisplay() => Name;
    public int CompareTo(Genre? other) => throw new NotImplementedException();
    public bool Equals(Genre? other) => other is not null && Id == other.Id;
    public override bool Equals(object? obj) => Equals(obj as Genre);
    public override int GetHashCode() => Id.GetHashCode();
    
    public static bool operator ==(Genre left, Genre right) => left is null ? right is null : left.Equals(right);
    public static bool operator !=(Genre left, Genre right) => !(left == right);
    public static bool operator <(Genre left, Genre right) => left is null ? right is not null : left.CompareTo(right) < 0;
    public static bool operator <=(Genre left, Genre right) => left is null || left.CompareTo(right) <= 0;
    public static bool operator >(Genre left, Genre right) => left is not null && left.CompareTo(right) > 0;
    public static bool operator >=(Genre left, Genre right) => left is null ? right is null : left.CompareTo(right) >= 0;
}