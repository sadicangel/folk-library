using AutoMapper;

namespace FolkLibrary.Application.Interfaces;

public interface IMapTo<T>
{
    void MapTo(Profile profile) => profile.CreateMap(GetType(), typeof(T));
}
