using System.Text.Json.Serialization;

namespace FolkLibrary;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Genre
{
    Folk
}