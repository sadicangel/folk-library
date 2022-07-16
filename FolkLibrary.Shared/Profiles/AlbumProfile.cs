using AutoMapper;
using FolkLibrary.Dtos;
using FolkLibrary.Models;

namespace FolkLibrary.Profiles;
public sealed class AlbumProfile : Profile
{
    public AlbumProfile()
    {
        CreateMap<Album, ItemReadDto>();
        CreateMap<Album, AlbumReadDto>();
    }
}
