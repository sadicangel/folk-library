using FolkLibrary.Interfaces;
using FolkLibrary.IO;
using FolkLibrary.Models;
using FolkLibrary.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Writers;
using System.Text.Json;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddFolkLibraryContext(this IServiceCollection services, string connectionString)
    {
        //services.AddEntityFrameworkNamingConventions();
        services.AddDbContextFactory<FolkLibraryContext>(opts => opts.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        return services;
    }
    public static IServiceCollection AddFolkLibraryClient(this IServiceCollection services, Action<HttpClient> configure)
    {
        services.AddHttpClient<IFolkLibraryClient, FolkLibraryClient>(configure);
        return services;
    }

    public static THost LoadDatabaseData<THost>(this THost host, bool overwrite = false) where THost : IHost
    {
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FolkLibraryContext>();
        if (!context.Database.CanConnect() || overwrite)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            var hostEnvironment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            var dataFolder = hostEnvironment.ContentRootFileProvider.GetFileInfo("data").PhysicalPath;
            foreach (var artistFolder in Directory.EnumerateDirectories(dataFolder))
            {
                var artistName = Path.GetFileName(artistFolder);
                Console.WriteLine(artistName);
                var singleArtist = default(Artist);
                var artists = new HashSet<Artist>();
                if (artistName != "Vários Artistas")
                {
                    singleArtist = JsonSerializer.Deserialize<Artist>(File.ReadAllText(Path.Combine(artistFolder, "info.json")), jsonOptions)!;
                    if (singleArtist.Name != artistName)
                        throw new AlbumReadException($"Folder '{artistName}' != ArtistName {singleArtist.Name}");
                    if (String.IsNullOrEmpty(singleArtist.ShortName))
                        throw new AlbumReadException($"Artist '{artistName}' does not have short name");
                    context.Artists.Add(singleArtist);
                    artists.Add(singleArtist);
                }
                foreach (var albumFolder in Directory.EnumerateDirectories(artistFolder))
                {
                    artists.Clear();
                    var albumName = Path.GetFileName(albumFolder);

                    var album = new Album
                    {
                        Name = albumName,
                        Year = null,
                        Description = null,
                        Genres = new HashSet<Genre> { Genre.Folk },
                        Tracks = new()
                    };
                    if (albumName.Length > 4 && Int32.TryParse(albumName.AsSpan()[^4..], out var albumYear))
                        album.Year = albumYear;

                    foreach (var trackFile in Directory.EnumerateFiles(albumFolder))
                    {
                        var meta = TagLib.File.Create(trackFile);
                        var tag = meta.Tag;

                        var track = new Track
                        {
                            Name = tag.Title,
                            Description = null,
                            Number = (int)tag.Track,
                            Year = tag.Year != 0 ? (int)tag.Year : null,
                            Duration = meta.Properties.Duration,
                            Genres = new HashSet<Genre> { Genre.Folk },
                        };

                        if (track.Name is null)
                            throw new AlbumReadException($"Track '{trackFile}' has no title");
                        if (track.Number == 0)
                            throw new AlbumReadException($"Track '{trackFile}' has no number");

                        if (singleArtist is not null)
                        {
                            if (tag.Performers.Length != 1)
                                throw new AlbumReadException($"Track '{track.Name}' must have a single artist. It has {tag.Performers.Length}");
                            if (tag.Performers[0] != singleArtist.Name)
                                throw new AlbumReadException($"Track artist '{tag.Performers[0]}' does not match artist '{singleArtist.Name}'");
                            singleArtist.Tracks.Add(track);
                        }
                        else
                        {
                            var variousArtists = context.Artists.Local.Where(a => meta.Tag.Performers.Contains(a.Name)).ToList();
                            if (variousArtists.Count == 0)
                                throw new AlbumReadException($"No artist for track {trackFile}");
                            foreach (var artist in variousArtists)
                            {
                                artist.Tracks.Add(track);
                                artists.Add(artist);
                            }
                        }

                        album.Tracks.Add(track);
                        album.TrackCount++;
                        album.Duration += track.Duration;
                    }

                    album.IsIncomplete = album.TrackCount != album.Tracks.Max(t => t.Number);

                    if (singleArtist is not null)
                    {
                        singleArtist.Albums.Add(album);
                    }
                    else
                    {
                        foreach (var artist in artists)
                            artist.Albums.Add(album);
                    }
                }
            }

            context.SaveChanges();
        }
        return host;
    }
}