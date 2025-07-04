namespace FolkLibrary.Domain.Albums;

public sealed record class AlbumCreated(
    Guid Id,
    string Name,
    string? Description,
    int? Year) : DomainEvent<Album>, ICreatedEvent
{
    public override Album Apply(Album aggregate)
    {
        aggregate.Id = Id;
        aggregate.Name = Name;
        aggregate.Description = Description;
        aggregate.Year = Year;
        aggregate.Artists = [];
        aggregate.Tracks = [];

        return aggregate;
    }
}
