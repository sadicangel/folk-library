using FolkLibrary.Services;
using MediatR;
using Spectre.Console;
using System.CommandLine;

namespace FolkLibrary.Artists;

internal sealed class QueryArtistsCommand : Command
{
    public QueryArtistsCommand() : base("get", "Query artists")
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
        AddOption(new Option<OrderBy>("--order-by", "Order artists by a column."));
    }
}

internal sealed record class QueryArtists
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
    string? OrderBy = null
) : IRequest<int>;

internal sealed class QueryArtistsHandler : IRequestHandler<QueryArtists, int>
{
    private static readonly Lazy<IReadOnlyDictionary<string, Func<Artist, string?>>> GettersLazy = new(CreateGetters);

    internal static IReadOnlyDictionary<string, Func<Artist, string?>> Getters { get => GettersLazy.Value; }

    private readonly IAnsiConsole _console;
    private readonly IFolkHttpClient _httpClient;

    public QueryArtistsHandler(IAnsiConsole console, IFolkHttpClient httpClient)
    {
        _console = console;
        _httpClient = httpClient;
    }

    public Task<int> Handle(QueryArtists request, CancellationToken cancellationToken) => _console.Unwrap(async () =>
    {
        IReadOnlyList<Artist> artists = Array.Empty<Artist>();
        if (request.Id is not null)
        {
            var artist = await _httpClient.GetArtistAsync(request.Id.Value, cancellationToken);
            if (artist is not null)
                artists = new List<Artist>() { artist };
        }
        else
        {
            var response = await _httpClient.GetArtistsAsync(
                request.Name,
                request.CountryCode,
                request.CountryName,
                request.District,
                request.Municipality,
                request.Parish,
                request.Year,
                request.AfterYear,
                request.BeforeYear,
                request.OrderBy is null ? null : OrderBy.Parse(request.OrderBy, null),
                cancellationToken);

            artists = response.Artists;
        }

        var table = new Table();
        foreach (var propertyName in Getters.Keys)
            table.AddColumn(propertyName);
        foreach (var artist in artists)
        {
            var cells = new Text[Getters.Count];
            int i = 0;
            foreach (var (_, getter) in Getters)
                cells[i++] = new Text(getter.Invoke(artist) ?? string.Empty);
            table.AddRow(cells);
        }

        _console.Write(table);

        return 0;
    });

    private static IReadOnlyDictionary<string, Func<Artist, string?>> CreateGetters()
    {
        return new Dictionary<string, Func<Artist, string?>>
        {
            ["Name"] = a => a.ShortName,
            ["Year"] = a => a.YearString,
            ["Country"] = a => a.Location?.CountryName,
            ["Albums"] = a => a.Albums.Count.ToString()
        };
    }
}