using Ardalis.Specification;
using FolkLibrary.Models;

namespace FolkLibrary.Interfaces;

public interface IPostgresRepository<T> : IRepositoryBase<T> where T : class, IDomainObject
{
    
}
