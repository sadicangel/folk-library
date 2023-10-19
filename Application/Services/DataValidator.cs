using Marten;
using Microsoft.Extensions.Logging;

namespace FolkLibrary.Services;

public interface IDataValidator
{
    Task ValidateAsync();
}

internal sealed class DataValidator : IDataValidator
{
    private readonly ILogger<DataValidator> _logger;
    private readonly IDocumentSession _dbSession;

    public DataValidator(ILogger<DataValidator> logger, IDocumentSession dbSession)
    {
        _logger = logger;
        _dbSession = dbSession;
    }

    public async Task ValidateAsync()
    {
        _logger.LogInformation("Validating data... ");
        var streamIds = _dbSession.Events.QueryAllRawEvents().Select(e => e.StreamId).Distinct().ToList();
        var artists = new List<Artist>();
        foreach (var streamId in streamIds)
            artists.Add(await _dbSession.Events.AggregateStreamAsync<Artist>(streamId) ?? throw new InvalidOperationException($"Stream {streamId} return 'null' artist"));

        _logger.LogInformation("Artists..: {artistCount}", artists.Count);
        _logger.LogInformation("Albums...: {albumCount}", artists.SelectMany(a => a.Albums).DistinctBy(a => a.AlbumId).Count());
        _logger.LogInformation("Tracks...: {trackCount}", artists.SelectMany(a => a.Albums.SelectMany(b => b.Tracks)).DistinctBy(c => c.TrackId).Count());

        _logger.LogInformation("Done!");
    }
}
