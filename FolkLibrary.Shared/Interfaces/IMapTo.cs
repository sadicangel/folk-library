using AutoMapper;

namespace FolkLibrary.Interfaces;

public interface IMapTo<T>
{
    void MapTo(Profile profile) => profile.CreateMap(GetType(), typeof(T));
}
