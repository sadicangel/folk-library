namespace FolkLibrary;

public sealed record class AlbumCreated(Album Album)
{
    public Album Apply() => Album;

    public Artist Apply(Artist artist) => artist with
    {
        Albums = artist.Albums.Add(Album)
    };
}