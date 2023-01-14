using FolkLibrary.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace FolkLibrary.Services;

internal sealed class FolkDataValidator
{
    private readonly ILogger<FolkDataValidator> _logger;
    private readonly FolkDbContext _dbContext;

    public FolkDataValidator(ILogger<FolkDataValidator> logger, FolkDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task ValidateDataAsync()
    {
        _logger.LogInformation("Validating data... ");
        var albumsWithoutArtist = await _dbContext.Albums.Where(a => a.Artists.Count == 0).ToListAsync();
        if (albumsWithoutArtist.Count > 0)
            throw new FolkDataLoadException($"Albums without artist:\n{string.Join('\n', albumsWithoutArtist.Select(t => t.Name))}");
        var tracksWithoutAlbum = await _dbContext.Tracks.Where(t => t.Album == null).ToListAsync();
        if (tracksWithoutAlbum.Count > 0)
            throw new FolkDataLoadException($"Tracks without album:\n{string.Join('\n', tracksWithoutAlbum.Select(t => t.Name))}");
        var tracksWithoutArtist = await _dbContext.Tracks.Where(t => t.Artists.Count == 0).ToListAsync();
        if (tracksWithoutArtist.Count > 0)
            throw new FolkDataLoadException($"Tracks without artist:\n{string.Join('\n', tracksWithoutArtist.Select(t => t.Name))}");
        _logger.LogInformation("Done!");

        _logger.LogInformation("Artists..: {artistCount}", _dbContext.Artists.Count());
        _logger.LogInformation("Albums...: {albumCount}", _dbContext.Albums.Count());
        _logger.LogInformation("Tracks...: {trackCount}", _dbContext.Tracks.Count());
    }
}
