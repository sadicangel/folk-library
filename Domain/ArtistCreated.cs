namespace FolkLibrary;

public sealed record class ArtistCreated(Artist Artist)
{
    public Artist Apply() => Artist;
}