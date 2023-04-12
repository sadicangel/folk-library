using System.ComponentModel.DataAnnotations.Schema;

namespace FolkLibrary;
public abstract class Entity
{
    [Column(Order = 0)]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [Column(Order = 1)]
    public required string Name { get; set; }

    [Column(Order = 2)]
    public string? Description { get; set; }

    [Column(Order = 3)]
    public int? Year { get; set; }

    [Column(Order = 4)]
    public bool IsYearUncertain { get; set; }

    [Column(Order = 5, TypeName = "json")]
    public HashSet<string> Genres { get; set; } = new();

    public override bool Equals(object? obj) => obj is Entity other && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();
}
