using FolkLibrary.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace FolkLibrary;
public abstract class Entity<TId> : IIdObject where TId : IId<TId>
{
    [Column(Order = 0)]
    public TId Id { get; init; } = TId.New();
    Guid IIdObject.Id { get => Id.Value; }

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

    public override bool Equals(object? obj) => obj is Entity<TId> other && Id.Equals(other.Id);

    public override int GetHashCode() => Id.GetHashCode();
}
