using FolkLibrary.Interfaces;
using System.Text.Json.Serialization;

namespace FolkLibrary.Dtos;

public abstract class DataTransferObject<TId> : IDataTransferObject
    where TId : IId<TId>
{
    [JsonPropertyOrder(-1)]
    public TId Id { get; set; } = default!;

    Guid IDataTransferObject.Id { get => Id.Value; }
}
