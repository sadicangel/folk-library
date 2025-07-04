using FolkLibrary.Domain;

namespace FolkLibrary.Domain.Albums;

public sealed record class AlbumUpdated(
    string? Name,
    string? Description,
    int? Year) : DomainEvent<Album>
{
    public override Album Apply(Album aggregate)
    {
        aggregate.Name = Name ?? aggregate.Name;
        aggregate.Description = Description ?? aggregate.Description;
        aggregate.Year = Year ?? aggregate.Year;

        return aggregate;
    }
}
