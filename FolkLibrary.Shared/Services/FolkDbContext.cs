using FolkLibrary.Albums;
using FolkLibrary.Artists;
using FolkLibrary.Tracks;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FolkLibrary.Services;

internal sealed class FolkDbContext : DbContext
{
    [NotNull] public DbSet<Artist>? Artists { get; set; }
    [NotNull] public DbSet<Album>? Albums { get; set; }
    [NotNull] public DbSet<Track>? Tracks { get; set; }

    public FolkDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        foreach (var idType in FolkExtensions.GetAllIdTypes())
            configurationBuilder.Properties(idType).HaveConversion(idType.GetNestedType("EfCoreValueConverter")!);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Album>().Navigation(e => e.Tracks).AutoInclude();
    }
}
