using System.Collections.Immutable;
using FolkLibrary.Domain.Albums;
using JasperFx.Events;
using Marten;
using Marten.Events.Projections;
using Marten.Patching;

namespace FolkLibrary.Domain.Artists;
public class ArtistViewProjection : IProjection
{
    public async Task ApplyAsync(IDocumentOperations operations, IReadOnlyList<IEvent> events, CancellationToken cancellationToken)
    {
        foreach (var @event in events)
        {
            switch (@event.Data)
            {
                case ArtistCreated e:
                    operations.Store(new ArtistView(
                        Id: e.Id,
                        Name: e.Name,
                        ShortName: e.ShortName,
                        LetterAvatar: e.GetLetterAvatar(),
                        Description: e.Description,
                        Year: e.Year,
                        IsYearUncertain: e.IsYearUncertain,
                        YearString: e.GetYearString(),
                        Location: e.Location,
                        Albums: []));
                    break;

                case ArtistUpdated e:
                    var patch = operations.Patch<ArtistView>(@event.StreamId);
                    if (e.Name is not null)
                    {
                        patch = patch.Set(x => x.Name, e.Name);
                    }
                    if (e.ShortName is not null)
                    {
                        patch = patch
                            .Set(x => x.ShortName, e.ShortName)
                            .Set(x => x.LetterAvatar, e.GetLetterAvatar(e.ShortName));
                    }
                    if (e.Description is not null)
                    {
                        patch = patch.Set(x => x.Description, e.Description);
                    }
                    if (e.Year is not null)
                    {
                        patch = patch.Set(x => x.Year, e.Year);
                        if (e.IsYearUncertain is not null)
                            patch = patch.Set(x => x.YearString, e.GetYearString(e.Year, e.IsYearUncertain.Value));
                    }
                    else if (e.IsYearUncertain is not null)
                    {
                        patch = patch.Set(x => x.IsYearUncertain, e.IsYearUncertain);
                    }
                    if (e.Location is not null)
                    {
                        patch = patch.Set(x => x.Location, e.Location);
                    }
                    break;
                case AlbumLinked e:
                    var album = await operations.LoadAsync<Album>(e.AlbumId, cancellationToken);
                    if (album is not null)
                    {
                        var albumView = AlbumView.FromAlbum(album);
                        operations.Patch<ArtistView>(@event.StreamId)
                            .AppendIfNotExists(x => x.Albums, albumView, a => a.Id == e.AlbumId);
                    }
                    break;
                case AlbumUnlinked e:
                    operations.Patch<ArtistView>(@event.StreamId)
                        .Remove(x => x.Albums, a => a.Id == e.AlbumId);
                    break;
            }
        }
    }
}

public sealed record ArtistView(
    Guid Id,
    string Name,
    string ShortName,
    string LetterAvatar,
    string? Description,
    int? Year,
    bool IsYearUncertain,
    string YearString,
    Location Location,
    ImmutableList<AlbumView> Albums);

public sealed record AlbumView(
    Guid Id,
    string Name,
    string? Description,
    int? Year,
    bool IsYearUncertain,
    bool IsIncomplete,
    TimeSpan Duration,
    ImmutableList<Track> Tracks,
    bool IsCompilation)
{
    public static AlbumView FromAlbum(Album album) => new(
        album.Id,
        album.Name,
        album.Description,
        album.Year,
        album.IsYearUncertain,
        album.IsIncomplete,
        album.Duration,
        album.Tracks,
        album.IsCompilation);
}
