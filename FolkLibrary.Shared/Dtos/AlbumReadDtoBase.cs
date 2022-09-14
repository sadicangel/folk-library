using System.Text.Json.Serialization;

namespace FolkLibrary.Dtos;

public class AlbumReadDtoBase : ItemReadDto
{
    [JsonPropertyOrder(-1)]
    public int TrackCount { get; set; }
    
    [JsonPropertyOrder(-1)]
    public TimeSpan Duration { get; set; }
    
    [JsonPropertyOrder(-1)]
    public bool IsIncomplete { get; set; }
}
