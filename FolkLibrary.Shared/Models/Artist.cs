using System.Diagnostics;

namespace FolkLibrary.Models;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Artist : Item, IEquatable<Artist>, IComparable<Artist>
{
    public string Country { get; set; } = null!;
    public string? District { get; set; }
    public string? Municipality { get; set; }
    public string? Parish { get; set; }
    public List<Album> Albums { get; set; } = null!;
    public List<Track> Tracks { get; set; } = null!;

    private string GetDebuggerDisplay() => Name;
    public bool Equals(Artist? other) => other is not null && Id == other.Id;
    public override bool Equals(object? obj) => Equals(obj as Artist);
    public override int GetHashCode() => Id.GetHashCode();
    public int CompareTo(Artist? other) => other is null ? 1 : Name.CompareTo(other.Name);

    public static bool operator ==(Artist left, Artist right) => left is null ? right is null : left.Equals(right);
    public static bool operator !=(Artist left, Artist right) => !(left == right);
    public static bool operator <(Artist left, Artist right) => left is null ? right is not null : left.CompareTo(right) < 0;
    public static bool operator <=(Artist left, Artist right) => left is null || left.CompareTo(right) <= 0;
    public static bool operator >(Artist left, Artist right) => left is not null && left.CompareTo(right) > 0;
    public static bool operator >=(Artist left, Artist right) => left is null ? right is null : left.CompareTo(right) >= 0;
}
