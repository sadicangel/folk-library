using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FolkLibrary.Models;

public abstract class Item
{
    [Column(Order = 0)]
    [JsonPropertyOrder(-4)]
    public Guid Id { get; init; }

    [Column(Order = 1)]
    [JsonPropertyOrder(-3)]
    public string Name { get; set; } = null!;

    [Column(Order = 2)]
    [JsonPropertyOrder(-2)]
    public string Type { get; set; }

    [Column(Order = 3)]
    [JsonPropertyOrder(-1)]
    public string? Description { get; set; }

    protected Item()
    {
        Type = GetType().Name;
    }
}
