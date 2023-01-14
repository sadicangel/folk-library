using FolkLibrary.Albums;
using FolkLibrary.Interfaces;
using FolkLibrary.Services;

namespace FolkLibrary.Repositories;

public interface IAlbumEntityRepository : IRepository<Album> { }

internal sealed class AlbumEntityRepository : PostgresRepository<AlbumId, Album>, IAlbumEntityRepository
{
    public AlbumEntityRepository(FolkDbContext context) : base(context)
    {
    }
}
