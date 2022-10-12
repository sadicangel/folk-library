using FolkLibrary.Interfaces;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using StronglyTypedIds;
using System.Diagnostics;

namespace FolkLibrary.Models;

[BsonSerializer()]
[StronglyTypedId(StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.SystemTextJson | StronglyTypedIdConverter.NewtonsoftJson | StronglyTypedIdConverter.EfCoreValueConverter)]
public readonly partial struct AlbumId : IId<AlbumId> { }

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Album : DomainObject<AlbumId>, IEquatable<Album>, IComparable<Album>
{
    public int TrackCount { get; set; }

    public TimeSpan Duration { get; set; }

    public bool IsIncomplete { get; set; }

    public HashSet<Artist> Artists { get; set; } = new();

    public List<Track> Tracks { get; set; } = new();

    private string GetDebuggerDisplay() => $"{Name} ({Duration:mm\\:ss})";
    public bool Equals(Album? other) => base.Equals(other);
    public override bool Equals(object? obj) => base.Equals(obj as Album);
    public override int GetHashCode() => base.GetHashCode();
    public int CompareTo(Album? other) => base.CompareTo(other);
}