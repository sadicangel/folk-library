using FolkLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FolkLibrary.Services;

public sealed class FolkLibraryContext : DbContext
{
    [NotNull] public DbSet<Artist>? Artists { get; set; }
    [NotNull] public DbSet<Album>? Albums { get; set; }
    [NotNull] public DbSet<Track>? Tracks { get; set; }

    public FolkLibraryContext(DbContextOptions options) : base(options)
    {
    }
}
