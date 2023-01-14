using FolkLibrary.Artists;
using FolkLibrary.Interfaces;
using MongoDB.Driver;

namespace FolkLibrary.Repositories;

public interface IArtistDocumentRepository : IRepository<ArtistDocument> { }

internal sealed class ArtistDocumentRepository : MongoRepository<ArtistId, ArtistDocument>, IArtistDocumentRepository
{
    public ArtistDocumentRepository(IMongoDatabase database) : base(database)
    {
    }
}