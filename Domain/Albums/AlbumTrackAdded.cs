namespace FolkLibrary.Albums;

public sealed record class AlbumTrackAdded(Track Track)
{
    public Album Apply(Album aggregate)
    {
        aggregate.Tracks.Add(Track);
        aggregate.Tracks.Sort((a, b) => a.Number.CompareTo(b.Number));
        return aggregate with
        {
            IsIncomplete = aggregate.Tracks.Count != aggregate.Tracks.Max(t => t.Number),
            Duration = aggregate.Duration + Track.Duration
        };
    }
}
