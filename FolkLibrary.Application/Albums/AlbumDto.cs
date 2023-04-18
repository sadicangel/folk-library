using AutoMapper;
using FolkLibrary.Application.Interfaces;
using FolkLibrary.Tracks;

namespace FolkLibrary.Albums;

public sealed class AlbumDto : IDocument, IMapFrom<Album>
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    public int? Year { get; init; }

    public bool IsYearUncertain { get; init; }

    public required string YearString { get; init; }

    public required List<string> Genres { get; init; }

    public required int TrackCount { get; init; }

    public required TimeSpan Duration { get; init; }

    public bool IsIncomplete { get; init; }

    public required List<TrackDto> Tracks { get; init; }

    void IMapFrom<Album>.MapFrom(Profile profile) => profile
        .CreateMap<Album, AlbumDto>()
        .ForMember(dst => dst.YearString, opts => opts.MapFrom(src => src.GetYearString()));
}
