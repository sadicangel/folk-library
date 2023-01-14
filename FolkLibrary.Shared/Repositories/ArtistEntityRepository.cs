using FolkLibrary.Artists;
using FolkLibrary.Interfaces;
using FolkLibrary.Services;

namespace FolkLibrary.Repositories;

public interface IArtistEntityRepository : IRepository<Artist> { }

internal sealed class ArtistEntityRepository : PostgresRepository<ArtistId, Artist>, IArtistEntityRepository
{
    public ArtistEntityRepository(FolkDbContext context) : base(context)
    {
    }
}
