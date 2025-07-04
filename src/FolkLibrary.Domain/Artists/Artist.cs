using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace FolkLibrary.Domain.Artists;

public sealed class Artist : Aggregate<Artist>
{
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string LetterAvatar { get; set; }
    public string? Description { get; set; }
    public int? Year { get; set; }
    public bool IsYearUncertain { get; set; }
    public string YearString { get; set; }
    public Location Location { get; set; }
    public ImmutableHashSet<Guid> Albums { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [JsonConstructor]
    private Artist() { }

    public Artist(Guid id, string name, string shortName, string? description, int? year, bool isYearUncertain, Location location) =>
        EnqueueAndApply(new ArtistCreated(id, name, shortName, description, year, isYearUncertain, location));
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    protected override Artist Apply(DomainEvent<Artist> @event) => @event.Apply(this);

    public Artist Update(string? name = null, string? shortName = null, string? description = null, int? year = null, bool? isYearUncertain = null, Location? location = null) =>
        EnqueueAndApply(new ArtistUpdated(name, shortName, description, year, isYearUncertain, location));

    public Artist LinkAlbum(Guid albumId) =>
        EnqueueAndApply(new AlbumLinked(albumId));

    public Artist UnlinkAlbum(Guid albumId) =>
        EnqueueAndApply(new AlbumUnlinked(albumId));
}
