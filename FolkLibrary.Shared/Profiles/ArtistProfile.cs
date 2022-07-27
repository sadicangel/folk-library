using AutoMapper;
using FolkLibrary.Dtos;
using FolkLibrary.Models;

namespace FolkLibrary.Profiles;

public sealed class ArtistProfile : Profile
{
    public ArtistProfile()
    {
        CreateMap<Artist, ItemReadDto>();
        CreateMap<Artist, ArtistReadDtoBase>();
        CreateMap<Artist, ArtistReadDto>();
    }
}
