using FluentValidation.Results;
using Marten;
using Microsoft.Extensions.Logging;

namespace FolkLibrary.Services;

public interface IDataValidator
{
    Task<ValidationResult> ValidateAsync(CancellationToken cancellationToken = default);
}

internal sealed class DataValidator : IDataValidator
{
    private readonly ILogger<DataValidator> _logger;
    private readonly IDocumentSession _documentSession;

    public DataValidator(ILogger<DataValidator> logger, IDocumentSession documentSession)
    {
        _logger = logger;
        _documentSession = documentSession;
    }

    public async Task<ValidationResult> ValidateAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validating data... ");

        var artists = await _documentSession.Query<Artist>().ToListAsync(cancellationToken);
        var albums = await _documentSession.Query<Album>().ToListAsync(cancellationToken);
        var tracks = await _documentSession.Query<Track>().ToListAsync(cancellationToken);
        _logger.LogInformation("Artists..: {count}", artists.Count);
        _logger.LogInformation("Albums...: {count}", albums.Count/*artists.SelectMany(a => a.Albums).DistinctBy(a => a.AlbumId).Count()*/);
        _logger.LogInformation("Tracks...: {count}", tracks.Count/*artists.SelectMany(a => a.Albums.SelectMany(b => b.Tracks)).DistinctBy(c => c.TrackId).Count()*/);

        var artistErrors = artists.SelectMany(Validate);

        foreach (var artistError in artistErrors)
            _logger.LogWarning("{propertyName}: {errorMessage}", artistError.PropertyName, artistError.ErrorMessage);

        var albumErrors = albums.SelectMany(Validate);

        foreach (var albumError in albumErrors)
            _logger.LogWarning("{propertyName}: {errorMessage}", albumError.PropertyName, albumError.ErrorMessage);

        var trackErrors = tracks.SelectMany(Validate);

        foreach (var trackError in trackErrors)
            _logger.LogWarning("{propertyName}: {errorMessage}", trackError.PropertyName, trackError.ErrorMessage);

        var result = new ValidationResult(artistErrors.Concat(albumErrors).Concat(trackErrors));

        if (result.IsValid)
            _logger.LogInformation("All data valid!");
        else
            _logger.LogInformation("Invalid data found");

        return result;
    }

    private static IEnumerable<ValidationFailure> Validate(Artist artist)
    {
        return Enumerable.Empty<ValidationFailure>();
    }

    private static IEnumerable<ValidationFailure> Validate(Album album)
    {
        if (album.Artists.Count == 0)
            yield return new ValidationFailure(nameof(Album.Artists), $"Album {album.Name} must have at least 1 artist");

        if (album.Tracks.Count == 0)
            yield return new ValidationFailure(nameof(Album.Tracks), $"Album {album.Name} must have at least 1 track");
    }

    private static IEnumerable<ValidationFailure> Validate(Track track)
    {
        if (track.AlbumId == default)
            yield return new ValidationFailure(nameof(Track.AlbumId), $"Track {track.Name} must have an album");
    }
}
