using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace FolkLibrary.Domain.Albums;

public sealed class Album : Aggregate<Album>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public int? Year { get; set; }
    public bool IsYearUncertain => Year is null;
    public bool IsIncomplete => Tracks.Count == 0 || Tracks.Count != Tracks.Max(t => t.Number);
    public TimeSpan Duration => Tracks.Aggregate(TimeSpan.Zero, (acc, val) => acc + val.Duration);
    public ImmutableHashSet<Guid> Artists { get; set; }
    public ImmutableList<Track> Tracks { get; set; }
    public bool IsCompilation => Artists is { Count: >= 2 };

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [JsonConstructor]
    private Album() { }

    public Album(Guid id, string name, string? description, int? year) =>
        EnqueueAndApply(new AlbumCreated(id, name, description, year));
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    protected override Album Apply(DomainEvent<Album> @event) => @event.Apply(this);

    public Album Update(string? name = null, string? description = null, int? year = null) =>
        EnqueueAndApply(new AlbumUpdated(name, description, year));

    public Album LinkArtist(Guid artistId) =>
        EnqueueAndApply(new ArtistLinked(artistId));

    public Album UnlinkArtist(Guid artistId) =>
        EnqueueAndApply(new ArtistUnlinked(artistId));

    public Album AddTrack(Track track) =>
        EnqueueAndApply(new TrackAdded(track));
}
