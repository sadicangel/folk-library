using FolkLibrary.Artists;
using MongoDB.Driver;

namespace FolkLibrary.Repositories;

internal sealed class ArtistViewRepository : MongoRepository<ArtistDto>, IArtistViewRepository
{
    public ArtistViewRepository(IMongoDatabase database) : base(database)
    {
    }
}