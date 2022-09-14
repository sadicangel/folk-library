using FolkLibrary.Models;
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
}
