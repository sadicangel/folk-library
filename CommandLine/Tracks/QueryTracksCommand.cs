using FolkLibrary.Services;
using MediatR;
using Spectre.Console;
using System.CommandLine;

namespace FolkLibrary.Tracks;

internal sealed class QueryTracksCommand : Command
{
    public QueryTracksCommand() : base("get", "Query tracks")
    {
        AddOption(new Option<Guid>("--id", "Find by id."));
        AddOption(new Option<string>("--name", "Filter by name."));
        AddOption(new Option<string>("--country-code", "Filter by country code."));
        AddOption(new Option<string>("--country-name", "Filter by country."));
        AddOption(new Option<string>("--district", "Filter by district."));
        AddOption(new Option<string>("--municipality", "Filter by municipality."));
        AddOption(new Option<string>("--parish", "Filter by parish."));
        AddOption(new Option<string>("--year", "Filter created at year."));
        AddOption(new Option<string>("--after-year", "Filter created at or after year."));
        AddOption(new Option<string>("--before-year", "Filter created at or before year."));
        AddOption(new Option<TimeSpan>("--above-duration", "Filter by greater than or equal to duration."));
        AddOption(new Option<TimeSpan>("--below-duration", "Filter by less than or equal to duration."));
        AddOption(new Option<OrderBy>("--order-by", "Order tracks by a column."));
    }
}

internal sealed record class QueryTracks
(
    Guid? Id = null,
    string? Name = null,
    string? CountryCode = null,
    string? CountryName = null,
    string? District = null,
    string? Municipality = null,
    string? Parish = null,
    int? Year = null,
    int? AfterYear = null,
    int? BeforeYear = null,
    TimeSpan? AboveDuration = null,
    TimeSpan? BelowDuration = null,
    string? OrderBy = null
) : IRequest<int>;

internal sealed class QueryTracksHandler : IRequestHandler<QueryTracks, int>
{
    private static readonly Lazy<IReadOnlyDictionary<string, Func<Track, string?>>> GettersLazy = new(CreateGetters);

    internal static IReadOnlyDictionary<string, Func<Track, string?>> Getters { get => GettersLazy.Value; }

    private readonly IAnsiConsole _console;
    private readonly IFolkHttpClient _httpClient;

    public QueryTracksHandler(IAnsiConsole console, IFolkHttpClient httpClient)
    {
        _console = console;
        _httpClient = httpClient;
    }

    public Task<int> Handle(QueryTracks request, CancellationToken cancellationToken) => _console.Unwrap(async () =>
    {
        IReadOnlyList<Track> tracks = Array.Empty<Track>();
        if (request.Id is not null)
        {
            var track = await _httpClient.GetTrackAsync(request.Id.Value, cancellationToken);
            if (track is not null)
                tracks = new List<Track>() { track };
        }
        else
        {
            var response = await _httpClient.GetTracksAsync(
                request.Name,
                request.Year,
                request.AfterYear,
                request.BeforeYear,
                request.AboveDuration,
                request.BelowDuration,
                request.OrderBy is null ? null : OrderBy.Parse(request.OrderBy, null),
                cancellationToken);

            tracks = response.Tracks;
        }

        var table = new Table();
        foreach (var propertyName in Getters.Keys)
            table.AddColumn(propertyName);
        foreach (var track in tracks)
        {
            var cells = new Text[Getters.Count];
            int i = 0;
            foreach (var (_, getter) in Getters)
                cells[i++] = new Text(getter.Invoke(track) ?? string.Empty);
            table.AddRow(cells);
        }

        _console.Write(table);

        return 0;
    });

    private static IReadOnlyDictionary<string, Func<Track, string?>> CreateGetters()
    {
        return new Dictionary<string, Func<Track, string?>>
        {
            ["Number"] = a => a.Number.ToString("00"),
            ["Name"] = a => a.Name,
            ["Year"] = a => a.Year?.ToString() ?? "?",
            ["Duration"] = a => a.Duration.ToString("""hh\:mm\:ss"""),
        };
    }
}