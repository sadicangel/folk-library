using Ardalis.Specification;
using FolkLibrary.Models;

namespace FolkLibrary.Specifications;

public static class ItemSpecificationBuilderExtensions
{
    public static ISpecificationBuilder<T> GetByName<T>(this ISpecificationBuilder<T> builder, string name)
        where T : Item
    {
        builder.Where(a => a.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

        return builder;
    }
}

public static class ArtistSpecificationBuilderExtensions
{
    public static ISpecificationBuilder<Artist> Configure(this ISpecificationBuilder<Artist> builder)
    {
        return builder.Include(a => a.Albums).ThenInclude(a => a.Tracks.OrderBy(t => t.Number)).Include(t => t.Tracks).AsNoTracking();
    }

    public static ISpecificationBuilder<Artist> GetAll(this ISpecificationBuilder<Artist> builder, string? country, string? district, string? municipality, string? parish)
    {
        builder.Configure();

        if (country is not null)
            builder.Where(a => a.Country == country);
        if (district is not null)
            builder.Where(a => a.District == district);
        if (municipality is not null)
            builder.Where(a => a.Municipality == municipality);
        if (parish is not null)
            builder.Where(a => a.Parish == parish);

        return builder;
    }
}

public static class AlbumSpecificationBuilderExtensions
{
    public static ISpecificationBuilder<Album> Configure(this ISpecificationBuilder<Album> builder)
    {
        return builder.Include(a => a.Artists).Include(t => t.Tracks).AsNoTracking();
    }


    public static ISpecificationBuilder<Album> GetAll(this ISpecificationBuilder<Album> builder, Guid? artistId = null)
    {
        builder.Configure();

        if (artistId is not null)
            builder.Where(a => a.Artists.Any(g => g.Id == artistId));

        return builder;
    }
}

public static class TrackSpecificationBuilderExtensions
{

    public static ISpecificationBuilder<Track> Configure(this ISpecificationBuilder<Track> builder)
    {
        return builder.Include(a => a.Album).Include(a => a.Artists).Include(a => a.Genres).AsNoTracking();
    }

    public static ISpecificationBuilder<Track> GetAll(this ISpecificationBuilder<Track> builder, Guid? albumId = null, Guid? artistId = null)
    {
        builder.Configure();

        if (albumId is not null)
            builder.Where(a => a.Album!.Id == albumId).OrderBy(t => t.Number);
        if (artistId is not null)
            builder.Where(a => a.Artists.Any(g => g.Id == artistId));

        return builder;
    }
}
