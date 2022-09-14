using System.Text.Json.Serialization;

namespace FolkLibrary.Dtos;

public class TrackReadDtoBase : ItemReadDto
{
    [JsonPropertyOrder(-1)]
    public int Number { get; set; }

    [JsonPropertyOrder(-1)]
    public TimeSpan Duration { get; set; }
}
