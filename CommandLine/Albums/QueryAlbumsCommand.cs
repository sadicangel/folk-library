using FolkLibrary.Services;
using MediatR;
using Spectre.Console;
using System.CommandLine;

namespace FolkLibrary.Albums;

internal sealed class QueryAlbumsCommand : Command
{
    public QueryAlbumsCommand() : base("get", "Query albums")
    {
        AddOption(new Option<Guid>("--id", "Find by id."));
        AddOption(new Option<string>("--name", "Filter by name."));
        AddOption(new Option<string>("--year", "Filter created at year."));
        AddOption(new Option<string>("--after-year", "Filter created at or after year."));
        AddOption(new Option<string>("--before-year", "Filter created at or before year."));
        AddOption(new Option<OrderBy>("--order-by", "Order albums by a column."));
    }
}

internal sealed record class QueryAlbums
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

internal sealed class QueryAlbumsHandler : IRequestHandler<QueryAlbums, int>
{
    private static readonly Lazy<IReadOnlyDictionary<string, Func<Album, string?>>> GettersLazy = new(CreateGetters);

    internal static IReadOnlyDictionary<string, Func<Album, string?>> Getters { get => GettersLazy.Value; }

    private readonly IAnsiConsole _console;
    private readonly IFolkHttpClient _httpClient;

    public QueryAlbumsHandler(IAnsiConsole console, IFolkHttpClient httpClient)
    {
        _console = console;
        _httpClient = httpClient;
    }

    public Task<int> Handle(QueryAlbums request, CancellationToken cancellationToken) => _console.Unwrap(async () =>
    {
        IReadOnlyList<Album> albums = Array.Empty<Album>();
        if (request.Id is not null)
        {
            var album = await _httpClient.GetAlbumAsync(request.Id.Value, cancellationToken);
            if (album is not null)
                albums = new List<Album>() { album };
        }
        else
        {
            var response = await _httpClient.GetAlbumsAsync(
                request.Name,
                request.Year,
                request.AfterYear,
                request.BeforeYear,
                request.OrderBy is null ? null : OrderBy.Parse(request.OrderBy, null),
                cancellationToken);

            albums = response.Albums;
        }

        var table = new Table();
        foreach (var propertyName in Getters.Keys)
            table.AddColumn(propertyName);
        foreach (var album in albums)
        {
            var cells = new Text[Getters.Count];
            int i = 0;
            foreach (var (_, getter) in Getters)
                cells[i++] = new Text(getter.Invoke(album) ?? string.Empty);
            table.AddRow(cells);
        }

        _console.Write(table);

        return 0;
    });

    private static IReadOnlyDictionary<string, Func<Album, string?>> CreateGetters()
    {
        return new Dictionary<string, Func<Album, string?>>
        {
            ["Name"] = a => a.Name,
            ["Year"] = a => a.Year?.ToString() ?? "?",
        };
    }
}