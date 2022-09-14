using FolkLibrary.Models;
using System.Text.Json.Serialization;

namespace FolkLibrary.Dtos;

public abstract class ItemReadDto
{
    [JsonPropertyOrder(-2)]
    public Guid Id { get; set; }

    [JsonPropertyOrder(-2)]
    public string Name { get; set; } = null!;

    [JsonIgnore, JsonPropertyOrder(-5)]
    public string Type { get; set; } = null!;

    [JsonPropertyOrder(-2)]
    public string? Description { get; set; }

    [JsonPropertyOrder(-2)]
    public int? Year { get; set; }

    [JsonPropertyOrder(-2)]
    public bool IsYearUncertain { get; set; }

    [JsonPropertyOrder(-2)]
    public HashSet<Genre> Genres { get; set; } = new();
}