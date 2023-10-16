using FolkLibrary.Albums;
using FolkLibrary.Database;

namespace FolkLibrary.Repositories;

internal sealed class AlbumRepository : PostgresRepository<Album>, IAlbumRepository
{
    public AlbumRepository(FolkDbContext context) : base(context)
    {
    }
}
