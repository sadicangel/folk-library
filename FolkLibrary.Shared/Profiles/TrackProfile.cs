using AutoMapper;
using FolkLibrary.Dtos;
using FolkLibrary.Models;

namespace FolkLibrary.Profiles;

public sealed class TrackProfile : Profile
{
    public TrackProfile()
    {
        CreateMap<Track, ItemReadDto>();
        CreateMap<Track, TrackReadDto>();
    }
}
