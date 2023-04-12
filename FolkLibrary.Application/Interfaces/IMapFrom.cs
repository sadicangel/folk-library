using AutoMapper;

namespace FolkLibrary.Application.Interfaces;
public interface IMapFrom<T>
{
    void MapFrom(Profile profile) => profile.CreateMap(typeof(T), GetType());
}