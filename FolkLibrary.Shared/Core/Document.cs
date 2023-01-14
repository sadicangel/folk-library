using FolkLibrary.Interfaces;
using System.Text.Json.Serialization;

namespace FolkLibrary;

public abstract class Document<TId> : IIdObject where TId : IId<TId>
{
    [JsonPropertyOrder(-1)]
    public TId Id { get; init; } = TId.New();

    Guid IIdObject.Id { get => Id.Value; }

    [JsonPropertyOrder(-1)]
    public string Name { get; set; } = null!;

    [JsonPropertyOrder(-1)]
    public string? Description { get; set; }

    [JsonPropertyOrder(-1)]
    public int? Year { get; set; }

    [JsonPropertyOrder(-1)]
    public bool IsYearUncertain { get; set; }

    [JsonPropertyOrder(-1)]
    public HashSet<Genre> Genres { get; set; } = new();

    public override bool Equals(object? obj) => Equals(obj as Document<TId>);

    public override int GetHashCode() => Id.GetHashCode();
}
