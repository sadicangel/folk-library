using FolkLibrary.Albums;
using FolkLibrary.Artists;
using FolkLibrary.Data;
using FolkLibrary.Tracks;
using MediatR;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace FolkLibrary;

internal sealed class Parser
{
    private readonly IMediator _mediator;
    private readonly RootCommand _rootCommand;

    public Parser(IMediator mediator)
    {
        _mediator = mediator;
        _rootCommand = new RootCommand
        {
            new Command("data", "Data operations")
            {
                new CopyCommand
                {
                    Handler = CommandHandler.Create<Copy, CancellationToken>(HandleOptions)
                },
            },
            new Command("artist", "Operations related to artists")
            {
                new QueryArtistsCommand
                {
                    Handler = CommandHandler.Create<QueryArtists, CancellationToken>(HandleOptions)
                }
            },
            new Command("album", "Operations related to albums")
            {
                new QueryAlbumsCommand
                {
                    Handler = CommandHandler.Create<QueryAlbums, CancellationToken>(HandleOptions)
                }
            },
            new Command("track", "Operations related to tracks")
            {
                new QueryTracksCommand
                {
                    Handler = CommandHandler.Create<QueryTracks, CancellationToken>(HandleOptions)
                }
            },
            //new ExitCommand
            //{
            //    Handler = CommandHandler.Create<Exit, CancellationToken>(HandleOptions)
            //}
        };
    }
    private Task<int> HandleOptions<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : IRequest<int>
    {
        return _mediator.Send(request, cancellationToken);
    }

    public Task<int> InvokeAsync(string commandLine, IConsole? console = null) => _rootCommand.InvokeAsync(commandLine, console);
}
