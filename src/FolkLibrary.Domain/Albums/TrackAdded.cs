namespace FolkLibrary.Domain.Albums;

public sealed record class TrackAdded(Track Track) : DomainEvent<Album>
{
    public override Album Apply(Album aggregate)
    {
        var builder = aggregate.Tracks.ToBuilder();
        builder.Add(Track);
        builder.Sort((a, b) => a.Number.CompareTo(b.Number));
        aggregate.Tracks = builder.ToImmutable();

        return aggregate;
    }
}
