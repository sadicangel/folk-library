using FolkLibrary.Interfaces;
using FolkLibrary.Models;
using FolkLibrary.Specifications;

namespace FolkLibrary.Commands.GraphQL;

public sealed class Query
{
    public async ValueTask<IList<Artist>> GetArtists([Service] IRepository<Artist> artistRepository, string? country = null, string? district = null, string? municipality = null, string? parish = null)
    {
        return await artistRepository.ListAsync(new GenericSpecification<Artist>(builder => builder.GetAll(country, district, municipality, parish)));
    }

    public async ValueTask<Artist?> GetArtistById([Service] IRepository<Artist> artistRepository, Guid id)
    {
        return await artistRepository.SingleOrDefaultAsync(new GenericSingleResultSpecification<Artist>(id, builder => builder.Configure()));
    }

    public async ValueTask<IList<Artist>> GetArtistsByName([Service] IRepository<Artist> artistRepository, string name)
    {
        return await artistRepository.ListAsync(new GenericSpecification<Artist>(builder => builder.Configure().GetByName(name)));
    }


    public async ValueTask<IList<Album>> GetAlbums([Service] IRepository<Album> albumRepository, Guid? albumId = null)
    {
        return await albumRepository.ListAsync(new GenericSpecification<Album>(builder => builder.GetAll(albumId)));
    }

    public async ValueTask<Album?> GetAlbumById([Service] IRepository<Album> albumRepository, Guid id)
    {
        return await albumRepository.SingleOrDefaultAsync(new GenericSingleResultSpecification<Album>(id, builder => builder.Configure()));
    }

    public async ValueTask<IList<Album>> GetAlbumByName([Service] IRepository<Album> albumRepository, string name)
    {
        return await albumRepository.ListAsync(new GenericSpecification<Album>(builder => builder.Configure().GetByName(name)));
    }


    public async ValueTask<IList<Track>> GetTracks([Service] IRepository<Track> trackRepository, Guid? albumId = null, Guid? artistId = null)
    {
        return await trackRepository.ListAsync(new GenericSpecification<Track>(builder => builder.GetAll(albumId, artistId)));
    }

    public async ValueTask<Track?> GetTrackById([Service] IRepository<Track> trackRepository, Guid id)
    {
        return await trackRepository.SingleOrDefaultAsync(new GenericSingleResultSpecification<Track>(id, builder => builder.Configure()));
    }

    public async ValueTask<IList<Track>> GetTrackByName([Service] IRepository<Track> trackRepository, string name)
    {
        return await trackRepository.ListAsync(new GenericSpecification<Track>(builder => builder.Configure().GetByName(name)));
    }
}
