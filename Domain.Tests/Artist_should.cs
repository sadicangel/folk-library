using AutoFixture;
using Marten;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace FolkLibrary;

[Collection(nameof(TestCollection))]
public sealed class Artist_should : IAsyncLifetime
{
    private readonly SharedFixture _sharedFixture;
    private readonly Fixture _autoFixture;
    private IHost _host = default!;
    private IMediator _mediator = default!;

    public Artist_should(SharedFixture sharedFixture)
    {
        _sharedFixture = sharedFixture;
        _autoFixture = new Fixture();
        _autoFixture.Customize(new ImmutableCollectionsCustomization());
    }

    public async Task InitializeAsync()
    {
        _host = await _sharedFixture.CreateHostAsync();
        _mediator = _host.Services.GetRequiredService<IMediator>();
    }

    public Task DisposeAsync() => _host.StopAsync();

    [Fact]
    public async Task Create_artist_on_artist_created_event()
    {
        var createArtist = _autoFixture
            .Build<CreateArtistCommand>()
            .With(c => c.Year, () => Random.Shared.Next(1900, 2101))
            .With(c => c.IsYearUncertain, true)
            .Create();

        var artistId = await _mediator.Send(createArtist).UnwrapAsync();

        var session = _host.Services.GetRequiredService<IDocumentSession>();
        var artist = await session.Events.AggregateStreamAsync<Artist>(artistId);

        PropertyMatcher.AssertPropertiesMatch(createArtist, artist);
    }

    [Fact]
    public async Task Update_artist_on_artist_updated_event()
    {
        var createArtist = _autoFixture
            .Build<CreateArtistCommand>()
            .With(c => c.Year, () => Random.Shared.Next(1900, 2101))
            .With(c => c.IsYearUncertain, true)
            .Create();

        var artistId = await _mediator.Send(createArtist).UnwrapAsync();

        var updateArtist = _autoFixture
            .Build<UpdateArtistInfoRequest>()
            .With(c => c.Year, () => Random.Shared.Next(1900, 2101))
            .With(c => c.IsYearUncertain, true)
            .Create();

        await _mediator.Send(new UpdateArtistInfoCommand(artistId, updateArtist)).UnwrapAsync();

        var session = _host.Services.GetRequiredService<IDocumentSession>();
        var artist = await session.Events.AggregateStreamAsync<Artist>(artistId);

        PropertyMatcher.AssertPropertiesMatch(updateArtist, artist);
    }

    [Fact]
    public async Task Update_artist_on_album_created_event()
    {
        var createArtist = _autoFixture
            .Build<CreateArtistCommand>()
            .With(c => c.Year, () => Random.Shared.Next(1900, 2101))
            .With(c => c.IsYearUncertain, true)
            .Create();

        var artistId = await _mediator.Send(createArtist).UnwrapAsync();

        var createAlbum = _autoFixture
            .Build<CreateAlbumCommand>()
            .With(a => a.Year, () => Random.Shared.Next(1900, 2101))
            .Create();

        var albumId = await _mediator.Send(createAlbum).UnwrapAsync();

        var addAlbum = new AddAlbumToArtistCommand(artistId, albumId);

        await _mediator.Send(addAlbum).UnwrapAsync();

        var session = _host.Services.GetRequiredService<IDocumentSession>();

        var artist = await session.Events.AggregateStreamAsync<Artist>(artistId);

        Assert.NotNull(artist);
        var actualAlbum = Assert.Single(artist.Albums);
        PropertyMatcher.AssertPropertiesMatch(createAlbum, actualAlbum);
    }

    [Fact]
    public async Task Update_artist_on_album_updated_event()
    {
        var createArtist = _autoFixture
            .Build<CreateArtistCommand>()
            .With(c => c.Year, () => Random.Shared.Next(1900, 2101))
            .With(c => c.IsYearUncertain, true)
            .Create();

        var artistId = await _mediator.Send(createArtist).UnwrapAsync();

        var createAlbum = _autoFixture
            .Build<CreateAlbumCommand>()
            .With(a => a.Year, () => Random.Shared.Next(1900, 2101))
            .Create();

        var albumId = await _mediator.Send(createAlbum).UnwrapAsync();

        await _mediator.Send(new AddAlbumToArtistCommand(artistId, albumId)).UnwrapAsync();

        var updateAlbum = _autoFixture
            .Build<UpdateAlbumRequest>()
            .With(a => a.Year, () => Random.Shared.Next(1900, 2101))
            .Create();

        await _mediator.Send(new UpdateAlbumCommand(albumId, updateAlbum)).UnwrapAsync();

        var session = _host.Services.GetRequiredService<IDocumentSession>();

        var artist = await session.Events.AggregateStreamAsync<Artist>(artistId);

        Assert.NotNull(artist);
        PropertyMatcher.AssertPropertiesMatch(updateAlbum, artist.Albums[0]);
    }

    [Fact]
    public async Task Update_artist_on_album_deleted_event()
    {
        var createArtist = _autoFixture
            .Build<CreateArtistCommand>()
            .With(c => c.Year, () => Random.Shared.Next(1900, 2101))
            .With(c => c.IsYearUncertain, true)
            .Create();

        var artistId = await _mediator.Send(createArtist).UnwrapAsync();

        var createAlbum = _autoFixture
            .Build<CreateAlbumCommand>()
            .With(a => a.Year, () => Random.Shared.Next(1900, 2101))
            .Create();

        var albumId = await _mediator.Send(createAlbum).UnwrapAsync();

        await _mediator.Send(new AddAlbumToArtistCommand(artistId, albumId)).UnwrapAsync();

        var updateAlbum = _autoFixture
            .Build<UpdateAlbumRequest>()
            .With(a => a.Year, () => Random.Shared.Next(1900, 2101))
            .Create();

        await _mediator.Send(new UpdateAlbumCommand(albumId, updateAlbum)).UnwrapAsync();

        var session = _host.Services.GetRequiredService<IDocumentSession>();

        var removeAlbum = new RemoveAlbumFromArtistCommand(artistId, albumId);

        await _mediator.Send(removeAlbum).UnwrapAsync();

        var artist = await session.Events.AggregateStreamAsync<Artist>(artistId);

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