using Ardalis.Specification;

namespace FolkLibrary.Interfaces;

public interface IPostgresRepository<T> : IRepositoryBase<T> where T : class, IDomainObject
{
    
}
