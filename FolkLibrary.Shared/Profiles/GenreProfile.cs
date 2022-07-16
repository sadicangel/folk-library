using AutoMapper;
using FolkLibrary.Dtos;
using FolkLibrary.Models;

namespace FolkLibrary.Profiles;

public sealed class GenreProfile : Profile
{
    public GenreProfile()
    {
        CreateMap<Genre, ItemReadDto>();
        CreateMap<Genre, GenreReadDto>();
    }
}
