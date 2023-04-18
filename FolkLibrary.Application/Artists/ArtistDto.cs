using AutoMapper;
using FolkLibrary.Albums;
using FolkLibrary.Application.Interfaces;

namespace FolkLibrary.Artists;

public sealed class ArtistDto : IDocument, IMapFrom<Artist>
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string ShortName { get; init; }

    public required string LetterAvatar { get; init; }

    public string? Description { get; init; }

    public int? Year { get; init; }

    public bool IsYearUncertain { get; init; }

    public required string YearString { get; init; }

    public required List<string> Genres { get; init; }

    public required string Country { get; init; }

    public string? District { get; init; }

    public string? Municipality { get; init; }

    public string? Parish { get; init; }

    public required string Location { get; init; }

    public bool IsAbroad { get; init; }

    public required List<AlbumDto> Albums { get; init; }

    void IMapFrom<Artist>.MapFrom(Profile profile) => profile
        .CreateMap<Artist, ArtistDto>()
        .ForMember(dst => dst.LetterAvatar, opts => opts.MapFrom(src => src.GetLetterAvatar()))
        .ForMember(dst => dst.YearString, opts => opts.MapFrom(src => src.GetYearString()))
        .ForMember(dst => dst.Location, opts => opts.MapFrom(src => src.GetLocation()));


}
