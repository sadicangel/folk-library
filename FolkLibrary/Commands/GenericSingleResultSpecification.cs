using Ardalis.Specification;
using FolkLibrary.Models;

namespace FolkLibrary.Commands;

public sealed class GenericSingleResultSpecification<T> : Specification<T>, ISingleResultSpecification<T>
    where T : Item
{
    public GenericSingleResultSpecification(Guid id, Action<ISpecificationBuilder<T>> configure)
    {
        configure(Query.Where(k => k.Id == id));
    }
}
