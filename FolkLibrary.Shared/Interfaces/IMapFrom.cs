using AutoMapper;

namespace FolkLibrary.Interfaces;
public interface IMapFrom<T>
{
    void MapFrom(Profile profile) => profile.CreateMap(typeof(T), GetType());
}
