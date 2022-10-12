using FolkLibrary.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace FolkLibrary.Models;

public abstract class DomainObject<TId> : IDomainObject, IEquatable<DomainObject<TId>>, IComparable<DomainObject<TId>>
    where TId : IId<TId>
{
    [Column(Order = 0)]
    public TId Id { get; init; } = IId<TId>.New();

    Guid IDomainObject.Id { get => Id.Value; }

    [Column(Order = 1)]
    public string Name { get; set; } = null!;

    [Column(Order = 2)]
    public string? Description { get; set; }
    
    [Column(Order = 3)]
    public int? Year { get; set; }
    
    [Column(Order = 4)]
    public bool IsYearUncertain { get; set; }

    [Column(Order = 5, TypeName = "json")]
    public HashSet<Genre> Genres { get; set; } = new();

    public bool Equals(DomainObject<TId>? other) => other is not null && Id.Equals(other.Id);

    public int CompareTo(DomainObject<TId>? other) => other is null ? -1 : Name.CompareTo(other.Name);

    public override bool Equals(object? obj) => Equals(obj as DomainObject<TId>);

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(DomainObject<TId>? left, DomainObject<TId>? right) => left is null ? right is null : left.Equals(right);

    public static bool operator !=(DomainObject<TId>? left, DomainObject<TId>? right) => !(left == right);

    public static bool operator <(DomainObject<TId>? left, DomainObject<TId>? right) => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(DomainObject<TId>? left, DomainObject<TId>? right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(DomainObject<TId>? left, DomainObject<TId>? right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(DomainObject<TId>? left, DomainObject<TId>? right) => left is null ? right is null : left.CompareTo(right) >= 0;
}
