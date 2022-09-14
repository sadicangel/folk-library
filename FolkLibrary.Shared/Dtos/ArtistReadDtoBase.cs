using System.Text.Json.Serialization;

namespace FolkLibrary.Dtos;

public class ArtistReadDtoBase : ItemReadDto
{
    [JsonPropertyOrder(-1)]
    public string ShortName { get; set; } = null!;

    [JsonPropertyOrder(-1)]
    public string Country { get; set; } = null!;

    [JsonPropertyOrder(-1)]
    public string? District { get; set; }

    [JsonPropertyOrder(-1)]
    public string? Municipality { get; set; }

    [JsonPropertyOrder(-1)]
    public string? Parish { get; set; }

    [JsonPropertyOrder(-1)]
    public bool IsAbroad { get; set; }

    [JsonPropertyOrder(-1)]
    public int AlbumCount { get; set; }
}
