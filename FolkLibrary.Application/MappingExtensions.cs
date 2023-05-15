using FolkLibrary.Albums;
using FolkLibrary.Artists;
using FolkLibrary.Tracks;

namespace FolkLibrary;
public static class MappingExtensions
{
    public static ArtistDto ToArtistDto(this Artist artist) => new()
    {
        Id = artist.Id,
        Name = artist.Name,
        ShortName = artist.ShortName,
        LetterAvatar = artist.GetLetterAvatar(),
        Description = artist.Description,
        Year = artist.Year,
        IsYearUncertain = artist.IsYearUncertain,
        YearString = artist.GetYearString(),
        Genres = artist.Genres,
        Country = artist.Country,
        District = artist.District,
        Municipality = artist.Municipality,
        Parish = artist.Parish,
        Location = artist.GetLocation(),
        IsAbroad = artist.IsAbroad,
        Albums = new(artist.Albums.Select(ToAlbumDto))
    };

    public static AlbumDto ToAlbumDto(this Album album) => new()
    {
        Id = album.Id,
        Name = album.Name,
        Description = album.Description,
        IsCompilation = album.IsCompilation,
        Year = album.Year,
        IsYearUncertain = album.IsYearUncertain,
        YearString = album.GetYearString(),
        Genres = album.Genres,
        TrackCount = album.TrackCount,
        Duration = album.Duration,
        IsIncomplete = album.IsIncomplete,
        Tracks = new(album.Tracks.Select(ToTrackDto))
    };

    public static TrackDto ToTrackDto(this Track track) => new()
    {
        Id = track.Id,
        Name = track.Name,
        Description = track.Description,
        Year = track.Year,
        IsYearUncertain = track.IsYearUncertain,
        Genres = track.Genres,
        Number = track.Number,
        Duration = track.Duration,
    };
}
