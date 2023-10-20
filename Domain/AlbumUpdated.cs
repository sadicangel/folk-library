namespace FolkLibrary;

public sealed record class AlbumUpdated(
    Guid AlbumId,
    string Name,
    string? Description,
    int? Year)
{
    public Album Apply(Album aggregate)
    {
        return aggregate with
        {
            Name = Name,
            Description = Description,
            Year = Year,
            IsYearUncertain = Year is null,
        };
    }

    public Artist Apply(Artist aggregate)
    {
        var index = aggregate.Albums.FindIndex(a => a.AlbumId == AlbumId);
        if (index >= 0)
            aggregate.Albums[index] = Apply(aggregate.Albums[index]);
        return aggregate;
    }
}