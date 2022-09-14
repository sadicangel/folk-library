using System.Text.Json.Serialization;

namespace FolkLibrary.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Genre
{
    Folk
}