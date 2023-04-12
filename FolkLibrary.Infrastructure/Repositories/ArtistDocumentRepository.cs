using FolkLibrary.Artists;
using FolkLibrary.Services;
using MongoDB.Driver;

namespace FolkLibrary.Repositories;

internal sealed class ArtistDocumentRepository : MongoRepository<ArtistDto>, IArtistViewRepository
{
    public ArtistDocumentRepository(IMongoDatabase database, IEncryptorProvider encryptorProvider) : base(database, encryptorProvider)
    {
    }
}