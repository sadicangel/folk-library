using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FolkLibrary.Models;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Artist : Item, IEquatable<Artist>, IComparable<Artist>
{
    public string ShortName { get; set; } = null!;

    [StringLength(maximumLength: 3, MinimumLength = 3)]
    public string Country { get; set; } = null!;

    public string? District { get; set; }

    public string? Municipality { get; set; }

    public string? Parish { get; set; }

    public bool IsAbroad { get; set; }

    public int AlbumCount { get; set; }

    public List<Album> Albums { get; set; } = new();

    public List<Track> Tracks { get; set; } = new();

    private string GetDebuggerDisplay() => Name;
    public bool Equals(Artist? other) => base.Equals(other);
    public override bool Equals(object? obj) => base.Equals(obj as Artist);
    public override int GetHashCode() => base.GetHashCode();
    public int CompareTo(Artist? other) => base.CompareTo(other);
}
