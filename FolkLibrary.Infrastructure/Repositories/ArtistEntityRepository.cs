using FolkLibrary.Artists;
using FolkLibrary.Database;
using FolkLibrary.Services;

namespace FolkLibrary.Repositories;

internal sealed class ArtistEntityRepository : PostgresRepository<Artist>, IArtistRepository
{
    public ArtistEntityRepository(FolkDbContext context, IEncryptorProvider encryptorProvider) : base(context, encryptorProvider)
    {
    }
}
