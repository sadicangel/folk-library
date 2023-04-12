﻿using FolkLibrary.Artists;
using FolkLibrary.Tracks;
using System.Diagnostics;

namespace FolkLibrary.Albums;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Album : Entity
{
    public required int TrackCount { get; set; }

    public required TimeSpan Duration { get; set; }

    public bool IsIncomplete { get; set; }

    public HashSet<Artist> Artists { get; set; } = new();

    public List<Track> Tracks { get; set; } = new();

    private string GetDebuggerDisplay() => $"{Name} ({Duration:mm\\:ss})";
}