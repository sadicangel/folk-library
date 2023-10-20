using MediatR;
using Microsoft.Extensions.Logging;

namespace FolkLibrary.Services;

public interface IDataValidator
{
    Task ValidateAsync(CancellationToken cancellationToken = default);
}

internal sealed class DataValidator : IDataValidator
{
    private readonly ILogger<DataValidator> _logger;
    private readonly IMediator _mediator;

    public DataValidator(ILogger<DataValidator> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task ValidateAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validating data... ");
        var response = await _mediator.Send(new GetArtistsCommand(), cancellationToken).UnwrapAsync();

        var artists = response.Artists;

        _logger.LogInformation("Artists..: {count}", artists.Count);
        _logger.LogInformation("Albums...: {count}", artists.SelectMany(a => a.Albums).DistinctBy(a => a.AlbumId).Count());
        _logger.LogInformation("Tracks...: {count}", artists.SelectMany(a => a.Albums.SelectMany(b => b.Tracks)).DistinctBy(c => c.TrackId).Count());

        _logger.LogInformation("Done!");
    }
}
