using FolkLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FolkLibrary.Services;

public sealed class FolkLibraryContext : DbContext
{
    [NotNull] public DbSet<Album>? Albums { get; set; }
    [NotNull] public DbSet<Artist>? Artists { get; set; }
    [NotNull] public DbSet<Track>? Tracks { get; set; }
    [NotNull] public DbSet<Genre>? Genres { get; set; }

    public FolkLibraryContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Artist>(t =>
        {
            t.HasKey(e => e.Id);
            t.Property(e => e.Name)
             .IsRequired();
            t.Property(e => e.Country)
             .IsRequired();
            t.Property(e => e.District);
            t.Property(e => e.Municipality);
            t.Property(e => e.Parish);
        });

        modelBuilder.Entity<Album>(t =>
        {
            t.HasKey(e => e.Id);
            t.Property(e => e.Name)
             .IsRequired();
            t.Property(e => e.Description);
            t.Property(e => e.Year);
            t.Property(e => e.TrackCount);
            t.Property(e => e.Duration);

            t.HasMany(e => e.Artists)
             .WithMany(e => e.Albums)
             .UsingEntity<AlbumArtist>();

            t.HasMany(e => e.Genres)
             .WithMany(e => e.Albums)
             .UsingEntity<AlbumGenre>();

            t.HasMany(e => e.Tracks)
             .WithOne(e => e.Album)
             .IsRequired();
        });

        modelBuilder.Entity<Track>(t =>
        {
            t.HasKey(e => e.Id);
            t.Property(e => e.Name)
             .IsRequired();
            t.Property(e => e.Description);
            t.Property(e => e.Number);
            t.Property(e => e.Duration);

            t.HasMany(e => e.Artists)
             .WithMany(e => e.Tracks)
             .UsingEntity<ArtistTrack>();

            t.HasMany(e => e.Genres)
             .WithMany(e => e.Tracks)
             .UsingEntity<TrackGenre>();
        });

        modelBuilder.Entity<Genre>(t =>
        {
            t.HasKey(e => e.Id);
            t.Property(e => e.Name)
             .IsRequired();

            t.HasMany(e => e.Albums)
             .WithMany(e => e.Genres)
             .UsingEntity<AlbumGenre>();

            t.HasMany(e => e.Tracks)
             .WithMany(e => e.Genres)
             .UsingEntity<TrackGenre>();
        });
    }
}
