using System.Text.Json.Serialization;

namespace FolkLibrary.Dtos;

public abstract class ItemReadDto
{
    [JsonPropertyOrder(-4)]
    public Guid Id { get; set; }

    [JsonPropertyOrder(-3)]
    public string Name { get; set; } = null!;

    [JsonIgnore, JsonPropertyOrder(-2)]
    public string Type { get; set; } = null!;

    [JsonPropertyOrder(-1)]
    public string? Description { get; set; }
}