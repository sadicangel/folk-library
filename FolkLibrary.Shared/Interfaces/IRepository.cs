using Ardalis.Specification;
using FolkLibrary.Models;

namespace FolkLibrary.Interfaces;

public interface IRepository<T> : IRepositoryBase<T> where T : Item
{
}
