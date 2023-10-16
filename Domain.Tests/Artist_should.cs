using AutoFixture;
using FolkLibrary;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace Domain.Tests;

[Collection(nameof(TestCollection))]
public sealed class Artist_should : IAsyncLifetime
{
    private readonly SharedFixture _sharedFixture;
    private readonly Fixture _autoFixture;
    private IHost _host = default!;

    public Artist_should(SharedFixture sharedFixture)
    {
        _sharedFixture = sharedFixture;
        _autoFixture = new Fixture();
        _autoFixture.Customize(new ImmutableCollectionsCustomization());
    }

    public async Task InitializeAsync() => _host = await _sharedFixture.CreateHostAsync();

    public Task DisposeAsync() => _host.StopAsync();

    [Fact]
    public async Task Create_artist_on_artist_created_event()
    {
        var session = _host.Services.GetRequiredService<IDocumentSession>();

        var artistCreated = _autoFixture.Create<ArtistCreated>();

        session.Events.StartStream(artistCreated.Id, artistCreated);
        await session.SaveChangesAsync();

        var artist = await session.Events.AggregateStreamAsync<Artist>(artistCreated.Id);

        PropertyMatcher.AssertPropertiesMatch(artistCreated, artist);
    }

    [Fact]
    public async Task Update_artist_on_artist_updated_event()
    {
        var session = _host.Services.GetRequiredService<IDocumentSession>();

        var artistCreated = _autoFixture.Create<ArtistCreated>();
        var artistUpdated = _autoFixture.Create<ArtistUpdated>();

        session.Events.StartStream(artistCreated.Id, artistCreated, artistUpdated);
        await session.SaveChangesAsync();

        var artist = await session.Events.AggregateStreamAsync<Artist>(artistCreated.Id);

        PropertyMatcher.AssertPropertiesMatch(artistUpdated, artist);
    }

    [Fact]
    public async Task Update_artist_on_album_created_event()
    {
        var session = _host.Services.GetRequiredService<IDocumentSession>();

        var artistCreated = _autoFixture.Create<ArtistCreated>();
        var albumCreated = _autoFixture.Create<AlbumCreated>();

        session.Events.StartStream(artistCreated.Id, artistCreated, albumCreated);
        await session.SaveChangesAsync();

        var artist = await session.Events.AggregateStreamAsync<Artist>(artistCreated.Id);

        Assert.NotNull(artist);
        PropertyMatcher.AssertPropertiesMatch(albumCreated.Album, artist.Albums[0]);
    }

    [Fact]
    public async Task Update_artist_on_album_updated_event()
    {
        var session = _host.Services.GetRequiredService<IDocumentSession>();

        var artistCreated = _autoFixture.Create<ArtistCreated>();
        var albumCreated = _autoFixture.Create<AlbumCreated>();
        var albumUpdated = new AlbumUpdated(_autoFixture.Create<Album>() with { Id = albumCreated.Album.Id });

        session.Events.StartStream(artistCreated.Id, artistCreated, albumCreated, albumUpdated);
        await session.SaveChangesAsync();

        var artist = await session.Events.AggregateStreamAsync<Artist>(artistCreated.Id);

        Assert.NotNull(artist);
        PropertyMatcher.AssertPropertiesMatch(albumUpdated.Album, artist.Albums[0]);
    }

    [Fact]
    public async Task Update_artist_on_album_deleted_event()
    {
        var session = _host.Services.GetRequiredService<IDocumentSession>();

        var artistCreated = _autoFixture.Create<ArtistCreated>();
        var albumCreated = _autoFixture.Create<AlbumCreated>();

        session.Events.StartStream(artistCreated.Id, artistCreated, albumCreated);
        await session.SaveChangesAsync();

        var artist = await session.Events.AggregateStreamAsync<Artist>(artistCreated.Id);

        Assert.NotNull(artist);
        PropertyMatcher.AssertPropertiesMatch(albumCreated.Album, artist.Albums[0]);

        var albumDeleted = new AlbumDeleted(albumCreated.Album);

        await session.Events.AppendOptimistic(artistCreated.Id, albumDeleted);
        await session.SaveChangesAsync();

        artist = await session.Events.AggregateStreamAsync<Artist>(artistCreated.Id);

        Assert.NotNull(artist);
        Assert.Empty(artist.Albums);
    }
}

file static class PropertyMatcher
{
    public static void AssertPropertiesMatch(object? expected, object? actual)
    {
        Assert.NotNull(expected);
        Assert.NotNull(actual);

        var expectedProperties = expected.GetType().GetProperties().ToDictionary(p => p.Name);
        var actualProperties = actual.GetType().GetProperties().ToDictionary(p => p.Name);

        var unmatching = new List<(string Name, string Reason)>();
        foreach (var (propertyName, expectedProperty) in expectedProperties)
        {
            if (!actualProperties.TryGetValue(propertyName, out var actualProperty))
            {
                unmatching.Add((propertyName, "Missing"));
                continue;
            }

            var expectedValue = expectedProperty.GetValue(expected);
            var actualValue = actualProperty.GetValue(actual);

            if (expectedProperty.PropertyType != actualProperty.PropertyType && expectedValue?.GetType() != actualValue?.GetType())
            {
                unmatching.Add((propertyName, $"Expected type '{expectedProperty.PropertyType}'. Actual type '{actualProperty.PropertyType}'"));
                continue;
            }
            if (!AreEqual(expectedValue, actualValue))
            {
                unmatching.Add((propertyName, $"Expected value '{expectedProperty.GetValue(expected)}'. Actual value '{actualProperty.GetValue(actual)}'"));
                continue;
            }
        }

        if (unmatching.Count > 0)
            Assert.Fail($"The following properties do not match:{Environment.NewLine}{String.Join(Environment.NewLine, unmatching.Select(e => $"{e.Name}: {e.Reason}"))}");

    }

    private static bool AreEqual(object? a, object? b)
    {
        return a switch
        {
            null => b is null,
            string stringA => stringA.Equals(b),
            IEnumerable enumerableA => AreEqual(enumerableA, b as IEnumerable),
            _ => a.Equals(b)
        };
    }

    private static bool AreEqual(IEnumerable enumerableA, IEnumerable? enumerableB)
    {
        if (enumerableB is null)
            return false;

        foreach (var itemA in enumerableA)
        {
            if (!Contains(enumerableB, itemA))
                return false;
        }

        return true;

        static bool Contains(IEnumerable enumerable, object? value)
        {
            foreach (var item in enumerable)
            {
                if (AreEqual(value, item))
                    return true;
            }
            return false;
        }
    }
}