using FolkLibrary.Artists;
using FolkLibrary.Database;

namespace FolkLibrary.Repositories;

internal sealed class ArtistRepository : PostgresRepository<Artist>, IArtistRepository
{
    public ArtistRepository(FolkDbContext context) : base(context)
    {
    }
}
