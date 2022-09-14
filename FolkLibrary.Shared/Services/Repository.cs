using Ardalis.Specification.EntityFrameworkCore;
using FolkLibrary.Interfaces;
using FolkLibrary.Models;

namespace FolkLibrary.Services;

internal sealed class Repository<T> : RepositoryBase<T>, IRepository<T>
    where T : Item
{
    public Repository(FolkDbContext context) : base(context)
    {
    }
}
