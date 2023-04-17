using FolkLibrary.Albums;
using FolkLibrary.Tracks;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FolkLibrary.Artists;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Artist : Entity
{
    public required string ShortName { get; set; }

    [StringLength(maximumLength: 2, MinimumLength = 2)]
    public required string Country { get; set; }

    public string? District { get; set; }

    public string? Municipality { get; set; }

    public string? Parish { get; set; }

    public bool IsAbroad { get; set; }

    public int AlbumCount { get => Albums.Count; }

    public List<Album> Albums { get; init; } = new();

    public int TracksCount { get => Tracks.Count; }

    public List<Track> Tracks { get; init; } = new();

    private string GetDebuggerDisplay() => Name;
}