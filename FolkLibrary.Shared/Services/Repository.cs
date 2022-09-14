using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FolkLibrary.Interfaces;
using FolkLibrary.Models;

namespace FolkLibrary.Services;

public sealed class Repository<T> : RepositoryBase<T>, IRepository<T>
    where T : Item
{
    public Repository(FolkLibraryContext context) : base(context)
    {
    }
}
