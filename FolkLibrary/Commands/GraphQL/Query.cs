using FolkLibrary.Models;
using FolkLibrary.Services;

namespace FolkLibrary.Commands.GraphQL;

public sealed class Query
{
    public async ValueTask<IList<Artist>> GetArtists([Service] IRepository<Artist> artistRepository, string? country = null, string? district = null, string? municipality = null, string? parish = null)
    {
        return await artistRepository.GetAllAsync(new GenericSpecification<Artist>(builder => builder.GetAll(country, district, municipality, parish)));
    }

    public async ValueTask<Artist?> GetArtistById([Service] IRepository<Artist> artistRepository, Guid id)
    {
        return await artistRepository.GetAsync(new GenericSingleResultSpecification<Artist>(id, builder => builder.Configure()));
    }

    public async ValueTask<IList<Artist>> GetArtistsByName([Service] IRepository<Artist> artistRepository, string name)
    {
        return await artistRepository.GetAllAsync(new GenericSpecification<Artist>(builder => builder.Configure().GetByName(name)));
    }


    public async ValueTask<IList<Album>> GetAlbums([Service] IRepository<Album> albumRepository, Guid? albumId = null, Guid? genreId = null)
    {
        return await albumRepository.GetAllAsync(new GenericSpecification<Album>(builder => builder.GetAll(albumId, genreId)));
    }

    public async ValueTask<Album?> GetAlbumById([Service] IRepository<Album> albumRepository, Guid id)
    {
        return await albumRepository.GetAsync(new GenericSingleResultSpecification<Album>(id, builder => builder.Configure()));
    }

    public async ValueTask<IList<Album>> GetAlbumByName([Service] IRepository<Album> albumRepository, string name)
    {
        return await albumRepository.GetAllAsync(new GenericSpecification<Album>(builder => builder.Configure().GetByName(name)));
    }


    public async ValueTask<IList<Genre>> GetGenres([Service] IRepository<Genre> genreRepository)
    {
        return await genreRepository.GetAllAsync(new GenericSpecification<Genre>(builder => builder.Configure()));
    }

    public async ValueTask<Genre?> GetGenreById([Service] IRepository<Genre> genreRepository, Guid id)
    {
        return await genreRepository.GetAsync(new GenericSingleResultSpecification<Genre>(id, builder => builder.Configure()));
    }

    public async ValueTask<IList<Genre>> GetGenreByName([Service] IRepository<Genre> genreRepository, string name)
    {
        return await genreRepository.GetAllAsync(new GenericSpecification<Genre>(builder => builder.Configure().GetByName(name)));
    }


    public async ValueTask<IList<Track>> GetTracks([Service] IRepository<Track> trackRepository, Guid? albumId = null, Guid? artistId = null, Guid? genreId = null)
    {
        return await trackRepository.GetAllAsync(new GenericSpecification<Track>(builder => builder.GetAll(albumId, artistId, genreId)));
    }

    public async ValueTask<Track?> GetTrackById([Service] IRepository<Track> trackRepository, Guid id)
    {
        return await trackRepository.GetAsync(new GenericSingleResultSpecification<Track>(id, builder => builder.Configure()));
    }

    public async ValueTask<IList<Track>> GetTrackByName([Service] IRepository<Track> trackRepository, string name)
    {
        return await trackRepository.GetAllAsync(new GenericSpecification<Track>(builder => builder.Configure().GetByName(name)));
    }
}
