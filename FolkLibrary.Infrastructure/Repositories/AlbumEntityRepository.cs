using FolkLibrary.Albums;
using FolkLibrary.Database;
using FolkLibrary.Services;

namespace FolkLibrary.Repositories;

internal sealed class AlbumEntityRepository : PostgresRepository<Album>, IAlbumRepository
{
    public AlbumEntityRepository(FolkDbContext context, IEncryptorProvider encryptorProvider) : base(context, encryptorProvider)
    {
    }
}
