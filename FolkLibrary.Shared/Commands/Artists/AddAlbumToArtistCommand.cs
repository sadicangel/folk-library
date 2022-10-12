using AutoMapper;
using FolkLibrary.Interfaces;
using FolkLibrary.Models;
using MediatR;

namespace FolkLibrary.Commands.Artists;

public sealed class CreateAlbumDto : IMapTo<Album>
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? Year { get; set; }

    public bool IsYearUncertain { get; set; }

    public HashSet<Genre> Genres { get; set; } = new();

    public TimeSpan Duration { get; set; }

    public bool IsIncomplete { get; set; }

    public List<CreateTrackDto> Tracks { get; set; } = null!;

    public void MapTo(Profile profile)
    {
        profile.CreateMap<CreateAlbumDto, Album>()
            .ForMember(e => e.TrackCount, e => e.MapFrom(e => e.Tracks.Count))
            .ForMember(e => e.Tracks, e => e.MapFrom(e => e.Tracks));
    }
}

public sealed class CreateTrackDto : IMapTo<Track>
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? Year { get; set; }

    public bool IsYearUncertain { get; set; }

    public HashSet<Genre> Genres { get; set; } = new();

    public int Number { get; set; }

    public TimeSpan Duration { get; set; }
}