namespace FolkLibrary;

public sealed record class AlbumUpdated(
    Guid AlbumId,
    string Name,
    string? Description,
    int? Year)
{
    public Artist Apply(Artist artist)
    {
        var index = artist.Albums.FindIndex(a => a.AlbumId == AlbumId);
        if (index >= 0)
        {
            artist.Albums[index] = artist.Albums[index] with
            {
                Name = Name,
                Description = Description,
                Year = Year
            };
        }
        return artist;
    }
}