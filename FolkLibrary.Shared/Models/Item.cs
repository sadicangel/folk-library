using System.ComponentModel.DataAnnotations.Schema;

namespace FolkLibrary.Models;

public abstract class Item : IEquatable<Item>, IComparable<Item>
{
    [Column(Order = 0)]
    public Guid Id { get; init; }

    [Column(Order = 1)]
    public string Name { get; set; } = null!;

    [NotMapped]
    public string Type { get; set; }

    [Column(Order = 2)]
    public string? Description { get; set; }
    
    [Column(Order = 3)]
    public int? Year { get; set; }
    
    [Column(Order = 4)]
    public bool IsYearUncertain { get; set; }

    [Column(Order = 5, TypeName = "json")]
    public HashSet<Genre> Genres { get; set; } = new();

    protected Item() => Type = GetType().Name;

    public bool Equals(Item? other) => other is not null && Type == other.Type && Id == other.Id;

    public int CompareTo(Item? other) => other is null ? -1 : Name.CompareTo(other.Name);

    public override bool Equals(object? obj) => Equals(obj as Item);

    public override int GetHashCode() => HashCode.Combine(Type, Id);

    public static bool operator ==(Item? left, Item? right) => left is null ? right is null : left.Equals(right);

    public static bool operator !=(Item? left, Item? right) => !(left == right);

    public static bool operator <(Item? left, Item? right) => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Item? left, Item? right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Item? left, Item? right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Item? left, Item? right) => left is null ? right is null : left.CompareTo(right) >= 0;
}
