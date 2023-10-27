namespace FolkLibrary.Albums;

public sealed record class AlbumInfoUpdated(
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
}
